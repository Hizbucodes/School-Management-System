namespace SchoolManagementSystem.API.Dtos
{
    public class GradeResponseDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentAdmissionNumber { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public decimal Score { get; set; }
        public string GradeLetter { get; set; }
    }
}
