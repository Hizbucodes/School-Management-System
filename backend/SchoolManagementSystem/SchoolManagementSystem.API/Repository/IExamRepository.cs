using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IExamRepository
    {
        Task<Exam?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Exam>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Exam> CreateAsync(Exam exam, CancellationToken cancellationToken = default);
        Task<Exam> UpdateAsync(Exam exam, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    
        Task<IEnumerable<Exam>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Exam>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Exam>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<Exam>> GetUpcomingExamsAsync(CancellationToken cancellationToken = default);
        Task<Exam?> GetExamWithStudentsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsForCourseOnDateAsync(Guid courseId, DateTime examDate, CancellationToken cancellationToken = default);
    }
}
