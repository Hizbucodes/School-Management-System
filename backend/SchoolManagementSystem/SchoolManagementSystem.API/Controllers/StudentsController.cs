using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStudentRepository _studentService;

        public StudentsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IStudentRepository studentRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this._studentService = studentRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-student")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentRegistrationDto dto)
        {
            var result = await _studentService.RegisterStudentAsync(dto);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, studentId = result.StudentId });
        }
    }
}
