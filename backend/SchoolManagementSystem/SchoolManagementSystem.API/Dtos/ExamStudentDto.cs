namespace SchoolManagementSystem.API.Dtos
{
    public class ExamStudentDto
    {
        public Guid StudentId { get; set; }
        public string AdmissionNumber { get; set; }
        public int? MarksObtained { get; set; }
        public bool IsAbsent { get; set; }
    }
}
