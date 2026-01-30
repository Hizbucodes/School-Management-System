namespace SchoolManagementSystem.API.Dtos
{
    public class WeeklyScheduleDto
    {
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public Dictionary<DayOfWeek, List<TimeTableResponseDto>> Schedule { get; set; } = new();
    }
}
