using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class StudentRelationshipDto
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [StringLength(20)]
        public string Relationship { get; set; }
    }
}
