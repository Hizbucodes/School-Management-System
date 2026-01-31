using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IStudentParentRepository
    {
        Task<StudentParent?> GetRelationshipAsync(Guid studentId, Guid parentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<StudentParent>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<StudentParent>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid studentId, Guid parentId, CancellationToken cancellationToken = default);
        Task<StudentParent> CreateAsync(StudentParent studentParent, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid studentId, Guid parentId, CancellationToken cancellationToken = default);
        Task CreateMultipleAsync(IEnumerable<StudentParent> studentParents, CancellationToken cancellationToken = default);
    }
}
