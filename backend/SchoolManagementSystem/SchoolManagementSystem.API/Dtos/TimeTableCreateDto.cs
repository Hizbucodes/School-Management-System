using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.API.Dtos
{
    public class TimeTableCreateDto
    {
        [Required]
        public Guid ClassId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        [Range(0, 6)]
        public DayOfWeek Day { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; }
    }
}
