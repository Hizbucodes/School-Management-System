using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Course> CreateAsync(Course course, CancellationToken cancellationToken = default);
        Task<Course> UpdateAsync(Course course, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<IEnumerable<Course>> GetCoursesByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Course>> GetCoursesByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}
