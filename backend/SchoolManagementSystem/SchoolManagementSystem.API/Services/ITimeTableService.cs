using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Services
{
    public interface ITimeTableService
    {
        Task<(bool Succeeded, string Message, Guid? TimeTableId)> CreateTimeTableAsync(TimeTableCreateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UpdateTimeTableAsync(Guid id, TimeTableUpdateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> DeleteTimeTableAsync(Guid id, CancellationToken cancellationToken = default);


        Task<TimeTableResponseDto?> GetTimeTableByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeTableResponseDto>> GetAllTimeTablesAsync(CancellationToken cancellationToken = default);
        Task<WeeklyScheduleDto?> GetClassWeeklyScheduleAsync(Guid classId, CancellationToken cancellationToken = default);
        Task<TeacherScheduleDto?> GetTeacherWeeklyScheduleAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<DailyScheduleDto> GetClassDailyScheduleAsync(Guid classId, DayOfWeek day, CancellationToken cancellationToken = default);
        Task<DailyScheduleDto> GetTeacherDailyScheduleAsync(Guid teacherId, DayOfWeek day, CancellationToken cancellationToken = default);
    }
}
