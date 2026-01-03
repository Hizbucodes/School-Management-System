namespace SchoolManagementSystem.API.Models
{
    public class Grade
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public decimal Score { get; set; }
        public string GradeLetter { get; set; }
    }
}
