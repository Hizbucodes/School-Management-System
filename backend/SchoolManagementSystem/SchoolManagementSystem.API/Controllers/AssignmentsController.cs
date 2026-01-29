using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly ICourseAssignmentService _assignmentService;

        public AssignmentsController(ICourseAssignmentService courseAssignmentService)
        {
            _assignmentService = courseAssignmentService;
        }



        [HttpPost("teachers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTeacher(Guid courseId, [FromBody] AssignTeacherDto dto, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _assignmentService.AssignTeacherToCourseAsync(courseId, dto.TeacherId, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [HttpPost("teachers/batch")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignMultipleTeachers(Guid courseId, [FromBody] AssignMultipleTeachersDto dto, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _assignmentService.AssignMultipleTeachersAsync(courseId, dto.TeacherIds, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [HttpDelete("teachers/{teacherId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveTeacher(Guid courseId, Guid teacherId, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _assignmentService.RemoveTeacherFromCourseAsync(courseId, teacherId, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [HttpPost("students")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollStudent(Guid courseId, [FromBody] EnrollStudentDto dto, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _assignmentService.EnrollStudentAsync(courseId, dto.StudentId, dto.AcademicYear, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [HttpPost("students/batch")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollMultipleStudents(Guid courseId, [FromBody] EnrollMultipleStudentsDto dto, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _assignmentService.EnrollMultipleStudentsAsync(courseId, dto.StudentIds, dto.AcademicYear, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }

   
        [HttpDelete("students/{studentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnenrollStudent(Guid courseId, Guid studentId, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _assignmentService.UnenrollStudentAsync(courseId, studentId, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }

    }
}
