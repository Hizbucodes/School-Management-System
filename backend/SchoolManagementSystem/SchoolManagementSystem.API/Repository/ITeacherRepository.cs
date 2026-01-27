using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface ITeacherRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Teacher> CreateAsync(Teacher teacher, CancellationToken cancellationToken = default);
        Task<Teacher?> UpdateAsync(Teacher teacher, CancellationToken cancellationToken = default);
        Task<Teacher?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Teacher>> GetAllAsync(CancellationToken cancellationToken = default);

    }
}
