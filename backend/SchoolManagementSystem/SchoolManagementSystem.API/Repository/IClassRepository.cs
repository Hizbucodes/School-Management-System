using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IClassRepository
    {
        Task<Class> CreateClassAsync(Class entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<Class>> GetAllClassesAsync(CancellationToken cancellationToken = default);
        Task<Class?> GetClassByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Class?> UpdateClassAsync(Guid id, Class entity, CancellationToken cancellationToken = default);
        Task<Class> DeleteClassAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string name, string year, CancellationToken cancellationToken = default);
    }
}
