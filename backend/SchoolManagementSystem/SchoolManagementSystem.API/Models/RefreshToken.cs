using Microsoft.AspNetCore.Identity;

namespace SchoolManagementSystem.API.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public IdentityUser User { get; set; }
    }
}
