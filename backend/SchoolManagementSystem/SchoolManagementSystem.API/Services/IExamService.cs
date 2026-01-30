using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Services
{
    public interface IExamService
    {
        Task<(bool Succeeded, string Message, Guid? ExamId)> CreateExamAsync(ExamCreateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UpdateExamAsync(Guid id, ExamUpdateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> DeleteExamAsync(Guid id, CancellationToken cancellationToken = default);

 
        Task<ExamResponseDto?> GetExamByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ExamDetailDto?> GetExamDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ExamResponseDto>> GetExamsByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExamResponseDto>> GetExamsByClassAsync(Guid classId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExamResponseDto>> GetUpcomingExamsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ExamScheduleDto>> GetExamScheduleAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}
