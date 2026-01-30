namespace SchoolManagementSystem.API.Dtos
{
    public class CourseGradeDto
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int CreditHours { get; set; }
        public decimal Score { get; set; }
        public string GradeLetter { get; set; }
    }
}
