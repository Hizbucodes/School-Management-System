using Microsoft.AspNetCore.Identity;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class ParentService : IParentService
    {
        private readonly IParentRepository _parentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentParentRepository _studentParentRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ParentService> _logger;

        public ParentService(
            IParentRepository parentRepository,
            IStudentRepository studentRepository,
            IStudentParentRepository studentParentRepository,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            ILogger<ParentService> logger)
        {
            _parentRepository = parentRepository;
            _studentRepository = studentRepository;
            _studentParentRepository = studentParentRepository;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Succeeded, string Message, Guid? ParentId)> RegisterParentAsync(
            ParentRegistrationDto dto,
            CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Check if email exists
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                {
                    return (false, "Email address is already registered.", null);
                }

                // Validate students if provided
                if (dto.Students != null && dto.Students.Any())
                {
                    foreach (var student in dto.Students)
                    {
                        if (!await _studentRepository.ExistsAsync(student.StudentId, cancellationToken))
                        {
                            return (false, $"Student with ID {student.StudentId} not found.", null);
                        }

                        // Validate relationship
                        if (!IsValidRelationship(student.Relationship))
                        {
                            return (false, $"Invalid relationship type: {student.Relationship}. Valid types are: Father, Mother, Guardian, Grandparent, Other.", null);
                        }
                    }
                }

                // Create Identity User
                var user = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                var identityResult = await _userManager.CreateAsync(user, dto.Password);
                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return (false, errors, null);
                }

                // Add Role
                var roleResult = await _userManager.AddToRoleAsync(user, "Parent");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to assign role: {errors}", null);
                }

                // Create Parent Entity
                var parent = new Parent
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = user.Id,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    Occupation = dto.Occupation
                };

                try
                {
                    await _context.Parents.AddAsync(parent, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    // Link to students if provided
                    if (dto.Students != null && dto.Students.Any())
                    {
                        var studentParents = dto.Students.Select(s => new StudentParent
                        {
                            StudentId = s.StudentId,
                            ParentId = parent.Id,
                            Relationship = s.Relationship
                        }).ToList();

                        await _studentParentRepository.CreateMultipleAsync(studentParents, cancellationToken);
                    }

                    await transaction.CommitAsync(cancellationToken);

                    return (true, $"Parent {parent.FullName} registered successfully.", parent.Id);
                }
                catch
                {
                    await _userManager.DeleteAsync(user);
                    throw;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while registering parent");
                return (false, "An internal error occurred during registration.", null);
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateParentAsync(
            Guid id,
            ParentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var parent = await _parentRepository.GetByIdAsync(id, cancellationToken);
                if (parent == null)
                {
                    return (false, "Parent not found.");
                }

                parent.FullName = dto.FullName;
                parent.PhoneNumber = dto.PhoneNumber;
                parent.Address = dto.Address;
                parent.Occupation = dto.Occupation;

                // Update phone number in Identity
                var user = await _userManager.FindByIdAsync(parent.IdentityUserId);
                if (user != null)
                {
                    user.PhoneNumber = dto.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                }

                await _parentRepository.UpdateAsync(parent, cancellationToken);

                return (true, "Parent updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating parent {ParentId}", id);
                return (false, "An error occurred while updating the parent.");
            }
        }

        public async Task<(bool Succeeded, string Message)> DeleteParentAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var parent = await _parentRepository.GetByIdAsync(id, cancellationToken);
                if (parent == null)
                {
                    return (false, "Parent not found.");
                }

                // Delete from database
                var deleted = await _parentRepository.DeleteAsync(id, cancellationToken);
                if (!deleted)
                {
                    return (false, "Failed to delete parent.");
                }

                // Delete Identity user
                var user = await _userManager.FindByIdAsync(parent.IdentityUserId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }

                return (true, "Parent deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting parent {ParentId}", id);
                return (false, "An error occurred while deleting the parent.");
            }
        }

        public async Task<ParentResponseDto?> GetParentByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var parent = await _parentRepository.GetByIdAsync(id, cancellationToken);
            if (parent == null)
                return null;

            var studentParents = await _studentParentRepository.GetByParentIdAsync(id, cancellationToken);

            return new ParentResponseDto
            {
                Id = parent.Id,
                FullName = parent.FullName,
                Email = parent.Email,
                PhoneNumber = parent.PhoneNumber,
                Address = parent.Address,
                Occupation = parent.Occupation,
                ChildrenCount = studentParents.Count()
            };
        }

        public async Task<ParentDetailDto?> GetParentDetailsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var parent = await _parentRepository.GetParentWithStudentsAsync(id, cancellationToken);
            if (parent == null)
                return null;

            return new ParentDetailDto
            {
                Id = parent.Id,
                FullName = parent.FullName,
                Email = parent.Email,
                PhoneNumber = parent.PhoneNumber,
                Address = parent.Address,
                Occupation = parent.Occupation,
                Children = parent.StudentParents?.Select(sp => new ParentStudentDto
                {
                    StudentId = sp.StudentId,
                    AdmissionNumber = sp.Student?.AdmissionNumber ?? "N/A",
                    ClassName = sp.Student?.Class?.Name ?? "N/A",
                    EnrollmentDate = sp.Student?.EnrollmentDate ?? DateTime.MinValue,
                    Relationship = sp.Relationship
                }).ToList() ?? new List<ParentStudentDto>()
            };
        }

        public async Task<IEnumerable<ParentResponseDto>> GetAllParentsAsync(
            CancellationToken cancellationToken = default)
        {
            var parents = await _parentRepository.GetAllAsync(cancellationToken);

            var parentDtos = new List<ParentResponseDto>();

            foreach (var parent in parents)
            {
                var studentParents = await _studentParentRepository.GetByParentIdAsync(parent.Id, cancellationToken);

                parentDtos.Add(new ParentResponseDto
                {
                    Id = parent.Id,
                    FullName = parent.FullName,
                    Email = parent.Email,
                    PhoneNumber = parent.PhoneNumber,
                    Address = parent.Address,
                    Occupation = parent.Occupation,
                    ChildrenCount = studentParents.Count()
                });
            }

            return parentDtos;
        }

        public async Task<IEnumerable<ParentResponseDto>> GetParentsByStudentIdAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var parents = await _parentRepository.GetParentsByStudentIdAsync(studentId, cancellationToken);

            return parents.Select(parent => new ParentResponseDto
            {
                Id = parent.Id,
                FullName = parent.FullName,
                Email = parent.Email,
                PhoneNumber = parent.PhoneNumber,
                Address = parent.Address,
                Occupation = parent.Occupation,
                ChildrenCount = parent.StudentParents?.Count ?? 0
            });
        }

        public async Task<(bool Succeeded, string Message)> LinkStudentToParentAsync(
            Guid parentId,
            Guid studentId,
            string relationship,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate parent exists
                if (!await _parentRepository.ExistsAsync(parentId, cancellationToken))
                {
                    return (false, "Parent not found.");
                }

                // Validate student exists
                if (!await _studentRepository.ExistsAsync(studentId, cancellationToken))
                {
                    return (false, "Student not found.");
                }

                // Validate relationship
                if (!IsValidRelationship(relationship))
                {
                    return (false, $"Invalid relationship type: {relationship}. Valid types are: Father, Mother, Guardian, Grandparent, Other.");
                }

                // Check if relationship already exists
                if (await _studentParentRepository.ExistsAsync(studentId, parentId, cancellationToken))
                {
                    return (false, "This student is already linked to this parent.");
                }

                var studentParent = new StudentParent
                {
                    StudentId = studentId,
                    ParentId = parentId,
                    Relationship = relationship
                };

                await _studentParentRepository.CreateAsync(studentParent, cancellationToken);

                return (true, "Student linked to parent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking student {StudentId} to parent {ParentId}", studentId, parentId);
                return (false, "An error occurred while linking student to parent.");
            }
        }

        public async Task<(bool Succeeded, string Message)> LinkMultipleStudentsAsync(
            Guid parentId,
            List<StudentRelationshipDto> students,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate parent exists
                if (!await _parentRepository.ExistsAsync(parentId, cancellationToken))
                {
                    return (false, "Parent not found.");
                }

                // Validate all students exist and relationships
                foreach (var student in students)
                {
                    if (!await _studentRepository.ExistsAsync(student.StudentId, cancellationToken))
                    {
                        return (false, $"Student with ID {student.StudentId} not found.");
                    }

                    if (!IsValidRelationship(student.Relationship))
                    {
                        return (false, $"Invalid relationship type: {student.Relationship}. Valid types are: Father, Mother, Guardian, Grandparent, Other.");
                    }
                }

                // Get existing relationships
                var existingRelationships = await _studentParentRepository.GetByParentIdAsync(parentId, cancellationToken);
                var existingStudentIds = existingRelationships.Select(sp => sp.StudentId).ToList();

                // Filter out already linked students
                var studentsToLink = students.Where(s => !existingStudentIds.Contains(s.StudentId)).ToList();

                if (!studentsToLink.Any())
                {
                    return (false, "All specified students are already linked to this parent.");
                }

                var studentParents = studentsToLink.Select(s => new StudentParent
                {
                    StudentId = s.StudentId,
                    ParentId = parentId,
                    Relationship = s.Relationship
                }).ToList();

                await _studentParentRepository.CreateMultipleAsync(studentParents, cancellationToken);

                return (true, $"{studentParents.Count} student(s) linked to parent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking multiple students to parent {ParentId}", parentId);
                return (false, "An error occurred while linking students to parent.");
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateRelationshipAsync(
            Guid parentId,
            Guid studentId,
            string relationship,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate relationship
                if (!IsValidRelationship(relationship))
                {
                    return (false, $"Invalid relationship type: {relationship}. Valid types are: Father, Mother, Guardian, Grandparent, Other.");
                }

                var studentParent = await _studentParentRepository.GetRelationshipAsync(studentId, parentId, cancellationToken);
                if (studentParent == null)
                {
                    return (false, "Student-parent relationship not found.");
                }

                // Update relationship
                studentParent.Relationship = relationship;

                _context.StudentParents.Update(studentParent);
                await _context.SaveChangesAsync(cancellationToken);

                return (true, "Relationship updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating relationship for student {StudentId} and parent {ParentId}", studentId, parentId);
                return (false, "An error occurred while updating the relationship.");
            }
        }

        public async Task<(bool Succeeded, string Message)> UnlinkStudentFromParentAsync(
            Guid parentId,
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _studentParentRepository.DeleteAsync(studentId, parentId, cancellationToken);

                if (!deleted)
                {
                    return (false, "Student-parent relationship not found.");
                }

                return (true, "Student unlinked from parent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking student {StudentId} from parent {ParentId}", studentId, parentId);
                return (false, "An error occurred while unlinking student from parent.");
            }
        }

        private bool IsValidRelationship(string relationship)
        {
            var validRelationships = new[] { "Father", "Mother", "Guardian", "Grandparent", "Other" };
            return validRelationships.Contains(relationship, StringComparer.OrdinalIgnoreCase);
        }
    }
}
