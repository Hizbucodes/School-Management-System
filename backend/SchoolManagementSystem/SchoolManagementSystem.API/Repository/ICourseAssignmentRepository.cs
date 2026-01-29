using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface ICourseAssignmentRepository
    {
    
        Task<TeacherCourse?> GetAssignmentAsync(Guid courseId, Guid teacherId, CancellationToken ct);
        Task AddTeacherCourseAsync(TeacherCourse tc, CancellationToken ct);
        Task AddTeacherCoursesBatchAsync(IEnumerable<TeacherCourse> tcs, CancellationToken ct);
        Task RemoveTeacherCourseAsync(TeacherCourse tc, CancellationToken ct);
        Task<List<Guid>> GetAssignedTeacherIdsAsync(Guid courseId, CancellationToken ct);

  

        Task<Enrollment?> GetEnrollmentAsync(Guid courseId, Guid studentId, CancellationToken ct);
        Task AddEnrollmentAsync(Enrollment enrollment, CancellationToken ct);
        Task AddEnrollmentsBatchAsync(IEnumerable<Enrollment> enrollments, CancellationToken ct);
        Task RemoveEnrollmentAsync(Enrollment enrollment, CancellationToken ct);
        Task<List<Guid>> GetEnrolledStudentIdsAsync(Guid courseId, CancellationToken ct);
    }
}
