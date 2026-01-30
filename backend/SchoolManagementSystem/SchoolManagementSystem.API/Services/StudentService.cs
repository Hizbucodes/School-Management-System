using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class StudentService : IStudentService
    {


        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Succeeded, string Message, Guid? StudentId)> RegisterStudentAsync(
             StudentRegistrationDto dto,
             CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Check if admission number exists
                if (await _studentRepository.ExistsByAdmissionNumberAsync(dto.AdmissionNumber, cancellationToken))
                {
                    return (false, "Admission number already exists.", null);
                }

                // 2. Check if email exists
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                {
                    return (false, "Email address is already registered.", null);
                }

                // 3. Validate class exists
                if (!await _classRepository.ExistAsync(dto.ClassId, cancellationToken))
                {
                    return (false, "The selected Class does not exist. Please provide a valid ClassId.", null);
                }

                // 4. Create Identity User
                var user = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email
                };

                var identityResult = await _userManager.CreateAsync(user, dto.Password);
                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return (false, errors, null);
                }

                // 5. Add Role
                var roleResult = await _userManager.AddToRoleAsync(user, "Student");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to assign role: {errors}", null);
                }

                // 6. Create Student Entity
                var student = new Student
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = user.Id,
                    AdmissionNumber = dto.AdmissionNumber,
                    EnrollmentDate = dto.EnrollmentDate,
                    ClassId = dto.ClassId
                };

                try
                {
                    await _context.Students.AddAsync(student, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return (true, $"Student {user.UserName} registered successfully.", student.Id);
                }
                catch
                {
                    // Clean up user if student creation fails
                    await _userManager.DeleteAsync(user);
                    throw;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while registering student");
                return (false, "An internal error occurred during registration.", null);
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateStudentAsync(
            Guid id,
            StudentUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
                if (student == null)
                {
                    return (false, "Student not found.");
                }

                // Validate class exists
                if (!await _classRepository.ExistAsync(dto.ClassId, cancellationToken))
                {
                    return (false, "The selected Class does not exist.");
                }

                student.ClassId = dto.ClassId;

                await _studentRepository.UpdateAsync(student, cancellationToken);

                return (true, "Student updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student {StudentId}", id);
                return (false, "An error occurred while updating the student.");
            }
        }

        public async Task<(bool Succeeded, string Message)> DeleteStudentAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
                if (student == null)
                {
                    return (false, "Student not found.");
                }

                // Delete from database
                var deleted = await _studentRepository.DeleteAsync(id, cancellationToken);
                if (!deleted)
                {
                    return (false, "Failed to delete student.");
                }

                // Delete Identity user
                var user = await _userManager.FindByIdAsync(student.IdentityUserId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }

                return (true, "Student deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student {StudentId}", id);
                return (false, "An error occurred while deleting the student.");
            }
        }

        public async Task<StudentResponseDto?> GetStudentByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
            if (student == null)
                return null;

            var user = await _userManager.FindByIdAsync(student.IdentityUserId);

            return new StudentResponseDto
            {
                Id = student.Id,
                AdmissionNumber = student.AdmissionNumber,
                Email = user?.Email ?? "N/A",
                EnrollmentDate = student.EnrollmentDate,
                ClassId = student.ClassId,
                ClassName = student.Class?.Name ?? "N/A"
            };
        }

        public async Task<StudentDetailDto?> GetStudentDetailsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
            if (student == null)
                return null;

            var user = await _userManager.FindByIdAsync(student.IdentityUserId);

            return new StudentDetailDto
            {
                Id = student.Id,
                AdmissionNumber = student.AdmissionNumber,
                Email = user?.Email ?? "N/A",
                PhoneNumber = user?.PhoneNumber ?? "N/A",
                EnrollmentDate = student.EnrollmentDate,
                ClassId = student.ClassId,
                ClassName = student.Class?.Name ?? "N/A",
                EnrolledCourses = student.Enrollments?.Select(e => new CourseEnrollmentDto
                {
                    CourseId = e.CourseId,
                    CourseName = e.Course?.Name ?? "N/A",
                    CourseCode = e.Course?.Code ?? "N/A",
                    AcademicYear = e.AcademicYear
                }).ToList() ?? new List<CourseEnrollmentDto>()
            };
        }

        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(
            CancellationToken cancellationToken = default)
        {
            var students = await _studentRepository.GetAllAsync(cancellationToken);

            var studentDtos = new List<StudentResponseDto>();

            foreach (var student in students)
            {
                var user = await _userManager.FindByIdAsync(student.IdentityUserId);

                studentDtos.Add(new StudentResponseDto
                {
                    Id = student.Id,
                    AdmissionNumber = student.AdmissionNumber,
                    Email = user?.Email ?? "N/A",
                    EnrollmentDate = student.EnrollmentDate,
                    ClassId = student.ClassId,
                    ClassName = student.Class?.Name ?? "N/A"
                });
            }

            return studentDtos;
        }

        public async Task<IEnumerable<StudentResponseDto>> GetStudentsByClassAsync(
            Guid classId,
            CancellationToken cancellationToken = default)
        {
            var students = await _studentRepository.GetByClassIdAsync(classId, cancellationToken);

            var studentDtos = new List<StudentResponseDto>();

            foreach (var student in students)
            {
                var user = await _userManager.FindByIdAsync(student.IdentityUserId);

                studentDtos.Add(new StudentResponseDto
                {
                    Id = student.Id,
                    AdmissionNumber = student.AdmissionNumber,
                    Email = user?.Email ?? "N/A",
                    EnrollmentDate = student.EnrollmentDate,
                    ClassId = student.ClassId,
                    ClassName = student.Class?.Name ?? "N/A"
                });
            }

            return studentDtos;
        }
    }
}
