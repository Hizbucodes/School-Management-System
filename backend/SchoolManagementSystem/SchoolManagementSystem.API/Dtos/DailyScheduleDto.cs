namespace SchoolManagementSystem.API.Dtos
{
    public class DailyScheduleDto
    {
        public DayOfWeek Day { get; set; }
        public string DayName { get; set; }
        public List<TimeTableResponseDto> Slots { get; set; } = new();
    }
}
