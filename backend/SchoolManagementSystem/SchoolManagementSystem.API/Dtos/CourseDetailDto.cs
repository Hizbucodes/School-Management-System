namespace SchoolManagementSystem.API.Dtos
{
    public class CourseDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CreditHours { get; set; }
        public List<AssignedTeacherDto> Teachers { get; set; } = new();
        public List<EnrolledStudentDto> Students { get; set; } = new();
    }
}
