namespace SchoolManagementSystem.API.Models
{
    public class Attendance
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid ClassId { get; set; }
        public Class Class { get; set; }


        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}
