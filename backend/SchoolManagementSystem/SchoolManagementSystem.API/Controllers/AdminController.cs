using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto dto)
        {
            // 1. Check if the role exists before creating the user
            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);

            Console.WriteLine(roleExists);
            if (!roleExists)
            {
                return BadRequest(new { message = $"Role '{dto.Role}' does not exist." });
            }

            // 2. Map DTO to IdentityUser
            var user = new IdentityUser
            {
                UserName = dto.UserName ?? dto.Email, // Fallback to Email if UserName is null
                Email = dto.Email
            };

            // 3. Create User
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 4. Assign Role
            await _userManager.AddToRoleAsync(user, dto.Role);

            return Ok(new { message = $"{dto.Role} created successfully", userId = user.Id });
        }
    }
}
