namespace SchoolManagementSystem.API.Dtos
{
    public class AttendanceResponseDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentAdmissionNumber { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}
