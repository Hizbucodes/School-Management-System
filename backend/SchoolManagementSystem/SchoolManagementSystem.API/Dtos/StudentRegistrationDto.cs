using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class StudentRegistrationDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }


   
        [Required]
        public string AdmissionNumber { get; set; }

        [Required]
        public Guid ClassId { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }


        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
