namespace SchoolManagementSystem.API.Dtos
{
    public class AttendanceStatsDto
    {
        public Guid StudentId { get; set; }
        public string StudentAdmissionNumber { get; set; }
        public int TotalClasses { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendancePercentage { get; set; }
    }
}
