namespace SchoolManagementSystem.API.Dtos
{
    public class TeacherUpdateDto
    {
        public string Specialization { get; set; }
        public double Salary { get; set; }
        public string DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
