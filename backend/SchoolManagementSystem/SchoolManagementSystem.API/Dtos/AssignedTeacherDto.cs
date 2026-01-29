namespace SchoolManagementSystem.API.Dtos
{
    public class AssignedTeacherDto
    {
        public Guid TeacherId { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
    }
}
