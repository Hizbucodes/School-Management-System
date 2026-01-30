using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTablesController : ControllerBase
    {
        private readonly ITimeTableService _timeTableService;
        private readonly ILogger<TimeTablesController> _logger;

        public TimeTablesController(
            ITimeTableService timeTableService,
            ILogger<TimeTablesController> logger)
        {
            _timeTableService = timeTableService;
            _logger = logger;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTimeTable(
            [FromBody] TimeTableCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, timeTableId) = await _timeTableService.CreateTimeTableAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return CreatedAtAction(
                nameof(GetTimeTableById),
                new { id = timeTableId },
                new { message, timeTableId });
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TimeTableResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTimeTables(CancellationToken cancellationToken)
        {
            var timeTables = await _timeTableService.GetAllTimeTablesAsync(cancellationToken);
            return Ok(timeTables);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TimeTableResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTimeTableById(Guid id, CancellationToken cancellationToken)
        {
            var timeTable = await _timeTableService.GetTimeTableByIdAsync(id, cancellationToken);

            if (timeTable == null)
                return NotFound(new { message = "Timetable entry not found." });

            return Ok(timeTable);
        }


        [HttpGet("class/{classId}/weekly")]
        [ProducesResponseType(typeof(WeeklyScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClassWeeklySchedule(Guid classId, CancellationToken cancellationToken)
        {
            var schedule = await _timeTableService.GetClassWeeklyScheduleAsync(classId, cancellationToken);

            if (schedule == null)
                return NotFound(new { message = "Class not found." });

            return Ok(schedule);
        }


        [HttpGet("teacher/{teacherId}/weekly")]
        [ProducesResponseType(typeof(TeacherScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeacherWeeklySchedule(Guid teacherId, CancellationToken cancellationToken)
        {
            var schedule = await _timeTableService.GetTeacherWeeklyScheduleAsync(teacherId, cancellationToken);

            if (schedule == null)
                return NotFound(new { message = "Teacher not found." });

            return Ok(schedule);
        }


        [HttpGet("class/{classId}/daily/{day}")]
        [ProducesResponseType(typeof(DailyScheduleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClassDailySchedule(
            Guid classId,
            DayOfWeek day,
            CancellationToken cancellationToken)
        {
            var schedule = await _timeTableService.GetClassDailyScheduleAsync(classId, day, cancellationToken);
            return Ok(schedule);
        }


        [HttpGet("teacher/{teacherId}/daily/{day}")]
        [ProducesResponseType(typeof(DailyScheduleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeacherDailySchedule(
            Guid teacherId,
            DayOfWeek day,
            CancellationToken cancellationToken)
        {
            var schedule = await _timeTableService.GetTeacherDailyScheduleAsync(teacherId, day, cancellationToken);
            return Ok(schedule);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTimeTable(
            Guid id,
            [FromBody] TimeTableUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _timeTableService.UpdateTimeTableAsync(id, dto, cancellationToken);

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
        public async Task<IActionResult> DeleteTimeTable(Guid id, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _timeTableService.DeleteTimeTableAsync(id, cancellationToken);

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
