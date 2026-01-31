using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class UpdateRelationshipDto
    {
        [Required]
        [StringLength(20)]
        public string Relationship { get; set; }
    }
}
