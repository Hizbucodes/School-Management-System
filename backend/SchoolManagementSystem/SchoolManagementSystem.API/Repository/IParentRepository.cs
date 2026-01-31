using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IParentRepository
    {
        Task<Parent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Parent?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Parent>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Parent> CreateAsync(Parent parent, CancellationToken cancellationToken = default);
        Task<Parent> UpdateAsync(Parent parent, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Parent?> GetParentWithStudentsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Parent>> GetParentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}
