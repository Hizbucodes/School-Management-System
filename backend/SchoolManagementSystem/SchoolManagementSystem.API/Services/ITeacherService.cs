using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Services
{
    public interface ITeacherService
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task<(bool Succeeded, string Message, Guid? TeacherId)> RegisterTeacherAsync(TeacherRegistrationDto dto, CancellationToken cancellationToken);
        Task<Teacher?> UpdateAsync(Guid id, TeacherUpdateDto dto, CancellationToken cancellationToken);
        Task<Teacher?> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<TeacherResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    }
}
