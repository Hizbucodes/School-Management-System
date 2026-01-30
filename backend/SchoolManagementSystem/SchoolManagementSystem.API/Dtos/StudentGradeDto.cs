using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class StudentGradeDto
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Score { get; set; }
    }
}
