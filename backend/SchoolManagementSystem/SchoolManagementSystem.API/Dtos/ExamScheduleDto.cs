namespace SchoolManagementSystem.API.Dtos
{
    public class ExamScheduleDto
    {
        public DateTime Date { get; set; }
        public List<ExamResponseDto> Exams { get; set; } = new();
    }
}
