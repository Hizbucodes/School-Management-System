using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class ClassUpdateDto
    {
        [Required(ErrorMessage = "Class name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Class name must be between 1 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Academic year is required")]
        [Range(2000, 2100, ErrorMessage = "Academic year must be between 2000 and 2100")]
        public string AcademicYear { get; set; }

        public Guid? ClassTeacherId { get; set; }
    }
}
