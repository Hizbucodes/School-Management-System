namespace SchoolManagementSystem.API.Dtos
{
    public class TeacherScheduleDto
    {
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public Dictionary<DayOfWeek, List<TimeTableResponseDto>> Schedule { get; set; } = new();
    }
}
