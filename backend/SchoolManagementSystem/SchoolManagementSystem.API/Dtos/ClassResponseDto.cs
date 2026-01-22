namespace SchoolManagementSystem.API.Dtos
{
    public class ClassResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AcademicYear { get; set; }
        public Guid? ClassTeacherId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
