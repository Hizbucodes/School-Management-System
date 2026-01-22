namespace SchoolManagementSystem.API.Dtos
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; }
        public string StudentName { get; set; }

        public string AdmissionNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
