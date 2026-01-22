using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class ClassCreateDto
    {
        [Required(ErrorMessage = "Class name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Class name must be between 1 and 100 characters")]
        public string Name { get; set; } // e.g., "10-A"

        [Required(ErrorMessage = "Academic year is required")]
        [Range(2000, 2100, ErrorMessage = "Academic year must be between 2000 and 2100")]
        public string AcademicYear { get; set; } // e.g., "2025/2026"

        public Guid? ClassTeacherId { get; set; } // optional classTeacher if assigned by school admin
    }
}
