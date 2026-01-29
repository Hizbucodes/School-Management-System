using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            IAttendanceService attendanceService,
            ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAttendance(
            [FromBody] AttendanceCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, attendanceId) = await _attendanceService.CreateAttendanceAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return CreatedAtAction(
                nameof(GetAttendanceById),
                new { id = attendanceId },
                new { message, attendanceId });
        }


        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkBulkAttendance(
            [FromBody] BulkAttendanceCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, recordsCreated) = await _attendanceService.MarkBulkAttendanceAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message, recordsCreated });
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AttendanceResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttendanceById(Guid id, CancellationToken cancellationToken)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id, cancellationToken);

            if (attendance == null)
                return NotFound(new { message = "Attendance record not found." });

            return Ok(attendance);
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AttendanceResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAttendance(CancellationToken cancellationToken)
        {
            var attendances = await _attendanceService.GetAllAttendanceAsync(cancellationToken);
            return Ok(attendances);
        }


        [HttpGet("student/{studentId}")]
        [ProducesResponseType(typeof(IEnumerable<AttendanceResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudentAttendance(Guid studentId, CancellationToken cancellationToken)
        {
            var attendances = await _attendanceService.GetStudentAttendanceAsync(studentId, cancellationToken);
            return Ok(attendances);
        }


        [HttpGet("class/{classId}")]
        [ProducesResponseType(typeof(IEnumerable<AttendanceResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClassAttendance(Guid classId, CancellationToken cancellationToken)
        {
            var attendances = await _attendanceService.GetClassAttendanceAsync(classId, cancellationToken);
            return Ok(attendances);
        }


        [HttpGet("course/{courseId}")]
        [ProducesResponseType(typeof(IEnumerable<AttendanceResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseAttendance(Guid courseId, CancellationToken cancellationToken)
        {
            var attendances = await _attendanceService.GetCourseAttendanceAsync(courseId, cancellationToken);
            return Ok(attendances);
        }


        [HttpGet("date/{date}")]
        [ProducesResponseType(typeof(IEnumerable<AttendanceResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAttendanceByDate(DateTime date, CancellationToken cancellationToken)
        {
            var attendances = await _attendanceService.GetAttendanceByDateAsync(date, cancellationToken);
            return Ok(attendances);
        }


        [HttpGet("report/course/{courseId}/date/{date}")]
        [ProducesResponseType(typeof(AttendanceReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseAttendanceReport(
            Guid courseId,
            DateTime date,
            CancellationToken cancellationToken)
        {
            var report = await _attendanceService.GetCourseAttendanceReportAsync(courseId, date, cancellationToken);

            if (report == null)
                return NotFound(new { message = "Course not found." });

            return Ok(report);
        }


        [HttpGet("stats/student/{studentId}")]
        [ProducesResponseType(typeof(AttendanceStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentAttendanceStats(
            Guid studentId,
            [FromQuery] Guid? courseId,
            CancellationToken cancellationToken)
        {
            var stats = await _attendanceService.GetStudentAttendanceStatsAsync(studentId, courseId, cancellationToken);

            if (stats == null)
                return NotFound(new { message = "Student not found." });

            return Ok(stats);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAttendance(
            Guid id,
            [FromBody] AttendanceUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _attendanceService.UpdateAttendanceAsync(id, dto, cancellationToken);

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
        public async Task<IActionResult> DeleteAttendance(Guid id, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _attendanceService.DeleteAttendanceAsync(id, cancellationToken);

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
