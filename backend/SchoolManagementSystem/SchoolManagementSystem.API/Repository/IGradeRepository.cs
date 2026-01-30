using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IGradeRepository
    {
        Task<Grade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Grade>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Grade> CreateAsync(Grade grade, CancellationToken cancellationToken = default);
        Task<Grade> UpdateAsync(Grade grade, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

       
        Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Grade>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<Grade?> GetStudentCourseGradeAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<bool> ExistsForStudentCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task CreateMultipleAsync(IEnumerable<Grade> grades, CancellationToken cancellationToken = default);
        Task<IEnumerable<Grade>> GetStudentGradesWithCoursesAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}
