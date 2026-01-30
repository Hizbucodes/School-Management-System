using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class BulkGradeCreateDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one student grade is required")]
        public List<StudentGradeDto> Grades { get; set; } = new();
    }
}
