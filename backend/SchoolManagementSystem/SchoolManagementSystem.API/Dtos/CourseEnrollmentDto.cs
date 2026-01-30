namespace SchoolManagementSystem.API.Dtos
{
    public class CourseEnrollmentDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string AcademicYear { get; set; }
    }
}
