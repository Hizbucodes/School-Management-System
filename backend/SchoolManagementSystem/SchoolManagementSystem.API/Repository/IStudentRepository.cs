using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IStudentRepository
    {
        Task<(bool Succeeded, string Message, Guid? StudentId)> RegisterStudentAsync(StudentRegistrationDto dto);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Student> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
