namespace SchoolManagementSystem.API.Models
{
    public class StudentParent
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid ParentId { get; set; }
        public Parent Parent { get; set; }

        
        public string Relationship { get; set; }  // Father, Mother, Guardian

    }
}
