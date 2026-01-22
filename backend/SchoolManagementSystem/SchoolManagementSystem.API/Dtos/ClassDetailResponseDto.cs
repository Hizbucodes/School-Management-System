using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Dtos
{
    public class ClassDetailResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AcademicYear { get; set; } 

        public Guid? ClassTeacherId { get; set; }
        public TeacherDto? ClassTeacher { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int StudentCount { get; set; }

        // FIX: Change these from Models to DTOs
        public ICollection<StudentDto> Students { get; set; } = new List<StudentDto>();

        public ICollection<TimeTableDto> TimeTables { get; set; } = new List<TimeTableDto>();
    }
}