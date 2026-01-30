using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class StudentUpdateDto
    {
        [Required]
        public Guid ClassId { get; set; }
    }
}
