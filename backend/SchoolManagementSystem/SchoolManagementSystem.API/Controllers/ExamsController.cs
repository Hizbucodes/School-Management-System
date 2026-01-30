using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILogger<ExamsController> _logger;

        public ExamsController(
            IExamService examService,
            ILogger<ExamsController> logger)
        {
            _examService = examService;
            _logger = logger;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExam(
            [FromBody] ExamCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, examId) = await _examService.CreateExamAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return CreatedAtAction(
                nameof(GetExamById),
                new { id = examId },
                new { message, examId });
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExamResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllExams(CancellationToken cancellationToken)
        {
            var exams = await _examService.GetAllExamsAsync(cancellationToken);
            return Ok(exams);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExamResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExamById(Guid id, CancellationToken cancellationToken)
        {
            var exam = await _examService.GetExamByIdAsync(id, cancellationToken);

            if (exam == null)
                return NotFound(new { message = "Exam not found." });

            return Ok(exam);
        }


        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ExamDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExamDetails(Guid id, CancellationToken cancellationToken)
        {
            var exam = await _examService.GetExamDetailsAsync(id, cancellationToken);

            if (exam == null)
                return NotFound(new { message = "Exam not found." });

            return Ok(exam);
        }


        [HttpGet("course/{courseId}")]
        [ProducesResponseType(typeof(IEnumerable<ExamResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExamsByCourse(Guid courseId, CancellationToken cancellationToken)
        {
            var exams = await _examService.GetExamsByCourseAsync(courseId, cancellationToken);
            return Ok(exams);
        }

      
        [HttpGet("class/{classId}")]
        [ProducesResponseType(typeof(IEnumerable<ExamResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExamsByClass(Guid classId, CancellationToken cancellationToken)
        {
            var exams = await _examService.GetExamsByClassAsync(classId, cancellationToken);
            return Ok(exams);
        }


        [HttpGet("upcoming")]
        [ProducesResponseType(typeof(IEnumerable<ExamResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUpcomingExams(CancellationToken cancellationToken)
        {
            var exams = await _examService.GetUpcomingExamsAsync(cancellationToken);
            return Ok(exams);
        }

        
        [HttpGet("schedule")]
        [ProducesResponseType(typeof(IEnumerable<ExamScheduleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExamSchedule(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            CancellationToken cancellationToken)
        {
            var schedule = await _examService.GetExamScheduleAsync(startDate, endDate, cancellationToken);
            return Ok(schedule);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateExam(
            Guid id,
            [FromBody] ExamUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _examService.UpdateExamAsync(id, dto, cancellationToken);

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteExam(Guid id, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _examService.DeleteExamAsync(id, cancellationToken);

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
