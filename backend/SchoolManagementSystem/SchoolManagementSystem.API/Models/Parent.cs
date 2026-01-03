namespace SchoolManagementSystem.API.Models
{
    public class Parent
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }

        public ICollection<StudentParent> StudentParents { get; set; }
    }
}
