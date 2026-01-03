namespace SchoolManagementSystem.API.Models
{
    public class Exam
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid ClassId { get; set; }
        public Class Class { get; set; }

        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int TotalMarks { get; set; }

        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}
