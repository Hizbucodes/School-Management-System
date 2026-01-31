using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class LinkMultipleStudentsDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one student is required")]
        public List<StudentRelationshipDto> Students { get; set; } = new();
    }
}
