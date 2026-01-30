using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class GradeUpdateDto
    {
        [Required]
        [Range(0, 100)]
        public decimal Score { get; set; }
    }
}
