using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class EnrollMultipleStudentsDto
    {
        [Required]
        [MinLength(1)]
        public List<Guid> StudentIds { get; set; }

        [Required]
        [StringLength(20)]
        public string AcademicYear { get; set; }
    }
}
