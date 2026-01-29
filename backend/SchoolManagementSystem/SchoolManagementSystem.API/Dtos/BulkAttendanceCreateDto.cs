using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class BulkAttendanceCreateDto
    {
        [Required]
        public Guid ClassId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one student attendance record is required")]
        public List<StudentAttendanceDto> Students { get; set; } = new();
    }
}
