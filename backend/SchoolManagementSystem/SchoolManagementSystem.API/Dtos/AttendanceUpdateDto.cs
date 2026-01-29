using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class AttendanceUpdateDto
    {
        [Required]
        public bool IsPresent { get; set; }
    }
}
