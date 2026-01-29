using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class AssignMultipleTeachersDto
    {
        [Required]
        [MinLength(1)]
        public List<Guid> TeacherIds { get; set; }
    }
}
