using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Services
{
    public interface ICourseService
    {
        Task<(bool Succeeded, string Message, Guid? CourseId)> CreateCourseAsync(CourseCreateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UpdateCourseAsync(Guid id, CourseUpdateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CourseResponseDto?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CourseDetailDto?> GetCourseDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CourseResponseDto>> GetAllCoursesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<CourseResponseDto>> GetCoursesByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CourseResponseDto>> GetCoursesByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}
