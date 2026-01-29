using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }



        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CourseResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCourses(CancellationToken cancellationToken)
        {
            var courses = await _courseService.GetAllCoursesAsync(cancellationToken);
            return Ok(courses);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseById(Guid id, CancellationToken cancellationToken)
        {
            var course = await _courseService.GetCourseByIdAsync(id, cancellationToken);

            if (course == null)
                return NotFound(new { message = "Course not found." });

            return Ok(course);
        }


        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseDetails(Guid id, CancellationToken cancellationToken)
        {
            var course = await _courseService.GetCourseDetailsAsync(id, cancellationToken);

            if (course == null)
                return NotFound(new { message = "Course not found." });

            return Ok(course);
        }


        [HttpGet("teacher/{teacherId}")]
        [ProducesResponseType(typeof(IEnumerable<CourseResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCoursesByTeacher(Guid teacherId, CancellationToken cancellationToken)
        {
            var courses = await _courseService.GetCoursesByTeacherAsync(teacherId, cancellationToken);
            return Ok(courses);
        }


        [HttpGet("student/{studentId}")]
        [ProducesResponseType(typeof(IEnumerable<CourseResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCoursesByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            var courses = await _courseService.GetCoursesByStudentAsync(studentId, cancellationToken);
            return Ok(courses);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can create courses
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, courseId) = await _courseService.CreateCourseAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return CreatedAtAction(
                nameof(GetCourseById),
                new { id = courseId },
                new { message, courseId });
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update courses
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CourseUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _courseService.UpdateCourseAsync(id, dto, cancellationToken);

            if (!succeeded)
            {
                if (message.Contains("not found"))
                    return NotFound(new { message });
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete courses
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _courseService.DeleteCourseAsync(id, cancellationToken);

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
