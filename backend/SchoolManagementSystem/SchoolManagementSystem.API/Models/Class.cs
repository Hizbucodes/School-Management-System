namespace SchoolManagementSystem.API.Models
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AcademicYear { get; set; }

        public Guid? ClassTeacherId { get; set; }
        public Teacher ClassTeacher { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int StudentCount { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
