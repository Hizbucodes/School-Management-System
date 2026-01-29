using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class AssignTeacherDto
    {
        [Required]
        public Guid TeacherId { get; set; }
    }
}
