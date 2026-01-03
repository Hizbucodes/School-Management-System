using System.Xml;

namespace SchoolManagementSystem.API.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CreditHours { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();
        public ICollection<Attendance> Attendances { get; set; }
    }
}
