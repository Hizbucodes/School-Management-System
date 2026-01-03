namespace SchoolManagementSystem.API.Models
{
    public class StudentExam
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }

        public int MarksObtained { get; set; }
        public bool IsAbsent { get; set; }
    }
}
