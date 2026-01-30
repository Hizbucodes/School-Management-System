using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        private readonly ILogger<GradesController> _logger;

        public GradesController(
            IGradeService gradeService,
            ILogger<GradesController> logger)
        {
            _gradeService = gradeService;
            _logger = logger;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGrade(
            [FromBody] GradeCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, gradeId) = await _gradeService.CreateGradeAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return CreatedAtAction(
                nameof(GetGradeById),
                new { id = gradeId },
                new { message, gradeId });
        }


        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBulkGrades(
            [FromBody] BulkGradeCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, gradesCreated) = await _gradeService.CreateBulkGradesAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message, gradesCreated });
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GradeResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllGrades(CancellationToken cancellationToken)
        {
            var grades = await _gradeService.GetAllGradesAsync(cancellationToken);
            return Ok(grades);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GradeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGradeById(Guid id, CancellationToken cancellationToken)
        {
            var grade = await _gradeService.GetGradeByIdAsync(id, cancellationToken);

            if (grade == null)
                return NotFound(new { message = "Grade not found." });

            return Ok(grade);
        }


        [HttpGet("student/{studentId}")]
        [ProducesResponseType(typeof(IEnumerable<GradeResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudentGrades(Guid studentId, CancellationToken cancellationToken)
        {
            var grades = await _gradeService.GetStudentGradesAsync(studentId, cancellationToken);
            return Ok(grades);
        }


        [HttpGet("course/{courseId}")]
        [ProducesResponseType(typeof(IEnumerable<GradeResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseGrades(Guid courseId, CancellationToken cancellationToken)
        {
            var grades = await _gradeService.GetCourseGradesAsync(courseId, cancellationToken);
            return Ok(grades);
        }


        [HttpGet("report-card/student/{studentId}")]
        [ProducesResponseType(typeof(StudentReportCardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentReportCard(Guid studentId, CancellationToken cancellationToken)
        {
            var reportCard = await _gradeService.GetStudentReportCardAsync(studentId, cancellationToken);

            if (reportCard == null)
                return NotFound(new { message = "Student not found." });

            return Ok(reportCard);
        }


        [HttpGet("statistics/course/{courseId}")]
        [ProducesResponseType(typeof(CourseGradeStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseGradeStats(Guid courseId, CancellationToken cancellationToken)
        {
            var stats = await _gradeService.GetCourseGradeStatsAsync(courseId, cancellationToken);

            if (stats == null)
                return NotFound(new { message = "Course not found." });

            return Ok(stats);
        }

  
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateGrade(
            Guid id,
            [FromBody] GradeUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _gradeService.UpdateGradeAsync(id, dto, cancellationToken);

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
        public async Task<IActionResult> DeleteGrade(Guid id, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _gradeService.DeleteGradeAsync(id, cancellationToken);

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
