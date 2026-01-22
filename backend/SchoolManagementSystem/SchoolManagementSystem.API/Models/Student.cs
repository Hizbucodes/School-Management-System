using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Security.Claims;

namespace SchoolManagementSystem.API.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; }



        public string AdmissionNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public Guid ClassId { get; set; }
        public Class Class { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<StudentParent> StudentParents { get; set; }
    }
}
