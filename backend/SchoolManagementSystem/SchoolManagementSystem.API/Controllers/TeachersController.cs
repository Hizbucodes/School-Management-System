using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Repository;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }




        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ClassDetailResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateTeacher(
            [FromBody] TeacherRegistrationDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _teacherService.RegisterTeacherAsync(dto, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.TeacherId },
                result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            var teachers = await _teacherService.GetAllAsync(cancellationToken);
            return Ok(teachers);
        }


        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var teacher = await _teacherService.GetByIdAsync(id, cancellationToken);
            return Ok(teacher);
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] TeacherUpdateDto dto,
            CancellationToken cancellationToken)
        {
            var updatedTeacher = await _teacherService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updatedTeacher);
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _teacherService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }


        [HttpGet("{id:guid}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CheckExists(
            Guid id,
            CancellationToken cancellationToken)
        {
            var exists = await _teacherService.ExistsAsync(id, cancellationToken);
            return Ok(exists);
        }

        //[HttpGet("by-subject/{subjectId:guid}")]
        //[ProducesResponseType(typeof(IEnumerable<TeacherDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> GetBySubject(
        //    Guid subjectId,
        //    CancellationToken cancellationToken)
        //{
        //    var teachers = await _teacherService.GetBySubjectAsync(subjectId, cancellationToken);
        //    return Ok(teachers);
        //}


        //[HttpPost("{id:guid}/subjects")]
        //[Authorize(Roles = "Admin")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<IActionResult> AssignSubject(
        //    Guid id,
        //    [FromBody] SubjectAssignmentDto dto,
        //    CancellationToken cancellationToken)
        //{
        //    await _teacherService.AssignSubjectAsync(id, dto, cancellationToken);
        //    return NoContent();
        //}
    }
}
