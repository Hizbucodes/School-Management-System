using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class EnrollStudentDto
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [StringLength(20)]
        public string AcademicYear { get; set; }
    }
}
