namespace SchoolManagementSystem.API.Dtos
{
    public class ParentStudentDto
    {
        public Guid StudentId { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Relationship { get; set; }
    }
}
