namespace SchoolManagementSystem.API.Dtos
{
    public class CourseResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CreditHours { get; set; }
        public int EnrolledStudentsCount { get; set; }
        public int AssignedTeachersCount { get; set; }
        public List<AssignedTeacherDto> AssignedTeachers { get; set; } = new();
    }
}
