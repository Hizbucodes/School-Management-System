namespace SchoolManagementSystem.API.Services
{
    public interface ICourseAssignmentService
    {
        Task<(bool Succeeded, string Message)> AssignTeacherToCourseAsync(Guid courseId, Guid teacherId, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> RemoveTeacherFromCourseAsync(Guid courseId, Guid teacherId, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> AssignMultipleTeachersAsync(Guid courseId, List<Guid> teacherIds, CancellationToken cancellationToken = default);


        Task<(bool Succeeded, string Message)> EnrollStudentAsync(Guid courseId, Guid studentId, string academicYear, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UnenrollStudentAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> EnrollMultipleStudentsAsync(Guid courseId, List<Guid> studentIds, string academicYear, CancellationToken cancellationToken = default);
    }
}
