using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Helpers;
using SchoolManagementSystem.API.Repository;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(
            IStudentService studentService,
            ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }


        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterStudent(
            [FromBody] StudentRegistrationDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, studentId) = await _studentService.RegisterStudentAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = studentId },
                new { message, studentId });
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudentResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStudents([FromQuery] QueryParameters parameters, CancellationToken cancellationToken)
        {
            var students = await _studentService.GetAllStudentsAsync( parameters, cancellationToken);
            return Ok(students);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentById(Guid id, CancellationToken cancellationToken)
        {
            var student = await _studentService.GetStudentByIdAsync(id, cancellationToken);

            if (student == null)
                return NotFound(new { message = "Student not found." });

            return Ok(student);
        }


        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(StudentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentDetails(Guid id, CancellationToken cancellationToken)
        {
            var student = await _studentService.GetStudentDetailsAsync(id, cancellationToken);

            if (student == null)
                return NotFound(new { message = "Student not found." });

            return Ok(student);
        }


        [HttpGet("class/{classId}")]
        [ProducesResponseType(typeof(IEnumerable<StudentResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudentsByClass(Guid classId, CancellationToken cancellationToken)
        {
            var students = await _studentService.GetStudentsByClassAsync(classId, cancellationToken);
            return Ok(students);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudent(
            Guid id,
            [FromBody] StudentUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _studentService.UpdateStudentAsync(id, dto, cancellationToken);

            if (!succeeded)
            {
                if (message.Contains("not found"))
                    return NotFound(new { message });
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(Guid id, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _studentService.DeleteStudentAsync(id, cancellationToken);

            if (!succeeded)
            {
                if (message.Contains("not found"))
                    return NotFound(new { message });
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}
