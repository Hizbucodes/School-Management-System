namespace SchoolManagementSystem.API.Dtos
{
    public class StudentResponseDto
    {
        public Guid Id { get; set; }
        public string AdmissionNumber { get; set; }
        public string Email { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
    }
}
