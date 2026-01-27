using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class TeacherRegistrationDto
    {
        // Account Details
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }

        // Teacher Details
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public double Salary { get; set; }
        [Required]
        public string DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;
    }
}
