using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Mappers;
using SchoolManagementSystem.API.Repository;
using SchoolManagementSystem.API.Services;

namespace SchoolManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly ILogger<ClassesController> _logger;

        public ClassesController(IClassService _classService, ILogger<ClassesController> logger)
        {
            this._classService = _classService;
            this._logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClassResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClassResponseDto>>> GetAllClasses(
              CancellationToken cancellationToken)
        {
            var classes = await _classService.GetAllClassesAsync(cancellationToken);

            return Ok(classes);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ClassDetailResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassDetailResponseDto>> GetClassById(
            Guid id,
            CancellationToken cancellationToken)
        {
       
                var schoolClass = await _classService.GetClassByIdAsync(id, cancellationToken);

                if (schoolClass is null)
                {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource Not Found",
                    Detail = $"Class with ID {id} not found",
                    Instance = HttpContext.Request.Path

                });
                }
          
                return Ok(schoolClass);
            }
      
        

        [HttpPost]
        [ProducesResponseType(typeof(ClassResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClassResponseDto>> CreateClass(
            [FromBody] ClassCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Exception handling by GlobalExceptionHandler
            var newClass = await _classService.CreateClassAsync(dto, cancellationToken);
      

            _logger.LogInformation(
                "Class created successfully: {ClassName} for year {AcademicYear} with ID {ClassId}",
                newClass.Name,
                newClass.AcademicYear,
                newClass.Id);

            return CreatedAtAction(
                nameof(GetClassById),
                new { id = newClass.Id },
                newClass);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateClass(
            Guid id,
            [FromBody] ClassUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _classService.UpdateClassAsync(id, dto, cancellationToken);

            _logger.LogInformation("Class with ID {ClassId} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteClass(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _classService.DeleteClassAsync(id, cancellationToken);

            _logger.LogInformation("Class with ID {ClassId} deleted successfully", id);
            return NoContent();
        }

    }
}
