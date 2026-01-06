using Microsoft.AspNetCore.Identity;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface ITokenService
    {
        string GenerateAccessToken(IdentityUser user, IList<string> roles);
        RefreshToken GenerateRefreshToken(string userId);
    }
}
