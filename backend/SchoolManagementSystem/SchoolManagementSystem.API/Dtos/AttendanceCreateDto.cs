using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class AttendanceCreateDto
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid ClassId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPresent { get; set; }
    }
}
