using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface ITeacherRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    }
}
