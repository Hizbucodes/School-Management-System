using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        private readonly IParentService _parentService;
        private readonly ILogger<ParentsController> _logger;

        public ParentsController(
            IParentService parentService,
            ILogger<ParentsController> logger)
        {
            _parentService = parentService;
            _logger = logger;
        }

  

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterParent(
            [FromBody] ParentRegistrationDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message, parentId) = await _parentService.RegisterParentAsync(dto, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return CreatedAtAction(
                nameof(GetParentById),
                new { id = parentId },
                new { message, parentId });
        }

    

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ParentResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllParents(CancellationToken cancellationToken)
        {
            var parents = await _parentService.GetAllParentsAsync(cancellationToken);
            return Ok(parents);
        }

   
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ParentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetParentById(Guid id, CancellationToken cancellationToken)
        {
            var parent = await _parentService.GetParentByIdAsync(id, cancellationToken);

            if (parent == null)
                return NotFound(new { message = "Parent not found." });

            return Ok(parent);
        }

       

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ParentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetParentDetails(Guid id, CancellationToken cancellationToken)
        {
            var parent = await _parentService.GetParentDetailsAsync(id, cancellationToken);

            if (parent == null)
                return NotFound(new { message = "Parent not found." });

            return Ok(parent);
        }



        [HttpGet("student/{studentId}")]
        [ProducesResponseType(typeof(IEnumerable<ParentResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetParentsByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            var parents = await _parentService.GetParentsByStudentIdAsync(studentId, cancellationToken);
            return Ok(parents);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateParent(
            Guid id,
            [FromBody] ParentUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _parentService.UpdateParentAsync(id, dto, cancellationToken);

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
        public async Task<IActionResult> DeleteParent(Guid id, CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _parentService.DeleteParentAsync(id, cancellationToken);

            if (!succeeded)
            {
                if (message.Contains("not found"))
                    return NotFound(new { message });
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }


        [HttpPost("{parentId}/students")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LinkStudentToParent(
            Guid parentId,
            [FromBody] LinkStudentToParentDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _parentService.LinkStudentToParentAsync(
                parentId,
                dto.StudentId,
                dto.Relationship,
                cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [HttpPost("{parentId}/students/batch")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LinkMultipleStudents(
            Guid parentId,
            [FromBody] LinkMultipleStudentsDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _parentService.LinkMultipleStudentsAsync(
                parentId,
                dto.Students,
                cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [HttpPut("{parentId}/students/{studentId}/relationship")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRelationship(
            Guid parentId,
            Guid studentId,
            [FromBody] UpdateRelationshipDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, message) = await _parentService.UpdateRelationshipAsync(
                parentId,
                studentId,
                dto.Relationship,
                cancellationToken);

            if (!succeeded)
            {
                if (message.Contains("not found"))
                    return NotFound(new { message });
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }


        [HttpDelete("{parentId}/students/{studentId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnlinkStudentFromParent(
            Guid parentId,
            Guid studentId,
            CancellationToken cancellationToken)
        {
            var (succeeded, message) = await _parentService.UnlinkStudentFromParentAsync(parentId, studentId, cancellationToken);

            if (!succeeded)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
