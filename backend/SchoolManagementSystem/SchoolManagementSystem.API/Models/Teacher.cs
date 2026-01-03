using System.Security.Claims;
using System.Xml;

namespace SchoolManagementSystem.API.Models
{
    public class Teacher
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; }

        public string Specialization { get; set; }
        public double Salary { get; set; }
        public string DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }

        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();
    }
}
