using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class StudentService : IStudentRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public StudentService(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<(bool Succeeded, string Message, Guid? StudentId)> RegisterStudentAsync(StudentRegistrationDto dto)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            if (await _context.Students.AnyAsync(s => s.AdmissionNumber == dto.AdmissionNumber))
                return (false, "Admission number already exists.", null);

            // 2. Check if email exists
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return (false, "Email address is already registered.", null);
            }

            var classExists = await _context.Classes.AnyAsync(c => c.Id == dto.ClassId);
            if (!classExists)
            {
                return (false, "The selected Class does not exist. Please provide a valid ClassId.", null);
            }


            try
            {
                // 2. Create Identity Account
                var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
                var identityResult = await _userManager.CreateAsync(user, dto.Password);

                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return (false, errors, null);
                }

                // 3. Add Role
                await _userManager.AddToRoleAsync(user, "Student");

                // 4. Create Student Entity (The record you showed earlier)
                var student = new Student
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = user.Id,
                    AdmissionNumber = dto.AdmissionNumber,
                    EnrollmentDate = dto.EnrollmentDate,
                    ClassId = dto.ClassId
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                // 5. Success
                await transaction.CommitAsync();
                return (true, $"Student {user.UserName} registered successfully within your school", student.Id);
            }
            catch (Exception ex)
            {
                // Roll back the transaction if either one fails -> (user creation or student entity creation)
                await transaction.RollbackAsync();
                // Log the error
                return (false, "An internal error occurred during registration.", null);
            }
        }
    }
}
