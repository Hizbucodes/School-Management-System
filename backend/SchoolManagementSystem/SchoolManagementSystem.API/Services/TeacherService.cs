using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Exceptions;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly AuthDbContext authDbContext;

        public TeacherService(UserManager<IdentityUser> userManager, ITeacherRepository teacherRepository, ApplicationDbContext applicationDbContext, AuthDbContext authDbContext)
        {
            this._userManager = userManager;
            this._teacherRepository = teacherRepository;
            this._applicationDbContext = applicationDbContext;
            this.authDbContext = authDbContext;
        }

        public async Task<(bool Succeeded, string Message, Guid? TeacherId)> RegisterTeacherAsync(TeacherRegistrationDto dto, CancellationToken cancellationToken)
        {
            // 1. Start Transaction
            using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

            try
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                {
                    return (false, $"Email address {dto.Email} is already registered.", null);
                }

                // 2. Create Identity Account
                var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
                var identityResult = await _userManager.CreateAsync(user, dto.Password);

                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return (false, errors, null);
                }

                // 3. Add Teacher Role
               var roleResult = await _userManager.AddToRoleAsync(user, "Teacher");
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to assign role: {errors}", null);
                }

                // 4. Create Teacher Entity
                var teacher = new Teacher
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = user.Id,
                    FullName = dto.FullName,
                    DateOfBirth = dto.DateOfBirth,
                    HireDate = dto.HireDate,
                    Salary = dto.Salary,
                    Specialization = dto.Specialization,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = dto.UpdatedAt
                    
                };

                await _teacherRepository.CreateAsync(teacher, cancellationToken);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                // 5. Commit
                await transaction.CommitAsync();
                return (true, $"Teacher {user.UserName} registered successfully.", teacher.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return (false, "An error occurred while creating the teacher account.", null);
            }
        }

        public async Task<Teacher?> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);

            if (teacher == null)
                throw new NotFoundException($"Teacher with ID {id} not found.");

            //Prevent deletion if teacher is assigned to a class
           bool isAssignedToClass = await _applicationDbContext.Classes.AnyAsync(c => c.ClassTeacherId == id, cancellationToken);

            if (isAssignedToClass)
            {
                throw new BusinessRuleViolationException("Cannot delete teacher because they are currently assigned as a Class Teacher.");
            }

            return await _teacherRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _teacherRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);

            if (teacher == null)
                throw new NotFoundException($"Teacher with ID {id} not found.");

            return teacher;
        }

        public async Task<Teacher?> UpdateAsync(Guid id, TeacherUpdateDto dto, CancellationToken cancellationToken)
        {
            // 1. Fetch existing teacher from repo (Tracking enabled)
            var existingTeacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);

            if (existingTeacher == null)
                throw new NotFoundException($"Teacher with ID {id} not found.");

            // 2. Update properties
            existingTeacher.Specialization = dto.Specialization;
            existingTeacher.UpdatedAt = dto.UpdatedAt;
            existingTeacher.HireDate = dto.HireDate;
            existingTeacher.DateOfBirth = dto.DateOfBirth;
            existingTeacher.Salary = dto.Salary;
            existingTeacher.CreatedAt = dto.CreatedAt;

            // 3. Save via repository
            return await _teacherRepository.UpdateAsync(existingTeacher, cancellationToken);
        }

        public async Task<IEnumerable<TeacherResponseDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var teachers = await _teacherRepository.GetAllAsync(cancellationToken);

            // Get all identity user IDs
            var identityUserIds = teachers.Select(t => t.IdentityUserId).ToList();

            var users = await authDbContext.Users
     .Where(u => identityUserIds.Contains(u.Id))
     .ToDictionaryAsync(u => u.Id, u => u, cancellationToken);

            return teachers.Select(t => new TeacherResponseDto
            {
                Id = t.Id,
                Specialization = t.Specialization,
                FullName = t.FullName ?? "N/A",
                Email = users.TryGetValue(t.IdentityUserId, out var user) ? user.Email : "N/A",
                CreatedAt = t.CreatedAt
            });
        }
    }
}
