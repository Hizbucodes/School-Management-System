namespace SchoolManagementSystem.API.Dtos
{
    public class ParentDetailDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Occupation { get; set; }
        public List<ParentStudentDto> Children { get; set; } = new();
    }
}
