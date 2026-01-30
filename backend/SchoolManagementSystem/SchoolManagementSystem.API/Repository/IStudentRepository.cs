using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Student?> GetByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default);
        Task<Student?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default);
        Task<Student> CreateAsync(Student student, CancellationToken cancellationToken = default);
        Task<Student> UpdateAsync(Student student, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default);
    }
}
