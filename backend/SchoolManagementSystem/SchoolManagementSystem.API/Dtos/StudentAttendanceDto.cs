using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class StudentAttendanceDto
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public bool IsPresent { get; set; }
    }
}
