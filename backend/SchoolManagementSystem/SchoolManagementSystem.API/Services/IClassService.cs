using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Services
{
    public interface IClassService
    {
        Task<ClassResponseDto> CreateClassAsync(ClassCreateDto dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync(CancellationToken cancellationToken = default);
        Task<ClassDetailResponseDto> GetClassByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ClassResponseDto> UpdateClassAsync(Guid id, ClassUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteClassAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
