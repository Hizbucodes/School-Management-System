namespace SchoolManagementSystem.API.Dtos
{
    public class CourseGradeStatsDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int TotalStudents { get; set; }
        public decimal AverageScore { get; set; }
        public decimal HighestScore { get; set; }
        public decimal LowestScore { get; set; }
        public int PassedStudents { get; set; }
        public int FailedStudents { get; set; }
        public double PassPercentage { get; set; }
        public GradeDistributionDto GradeDistribution { get; set; }
    }
}
