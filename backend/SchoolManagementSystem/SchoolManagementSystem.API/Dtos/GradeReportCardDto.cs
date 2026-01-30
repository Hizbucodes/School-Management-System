namespace SchoolManagementSystem.API.Dtos
{
    public class GradeReportCardDto
    {
        public Guid StudentId { get; set; }
        public string AdmissionNumber { get; set; }
        public string Email { get; set; }
        public string ClassName { get; set; }
        public List<CourseGradeDto> CourseGrades { get; set; } = new();
        public decimal OverallGPA { get; set; }
        public string OverallGrade { get; set; }
    }
}
