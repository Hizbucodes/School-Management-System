using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Exceptions;
using SchoolManagementSystem.API.Mappers;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ClassService(IClassRepository classRepository, ITeacherRepository teacherRepository, UserManager<IdentityUser> userManager)
        {
            this._classRepository = classRepository;
            this._teacherRepository = teacherRepository;
            this._userManager = userManager;
        }

        public async Task<ClassResponseDto> CreateClassAsync(ClassCreateDto dto, CancellationToken cancellationToken = default)
        {
            // 1. Validation: If a teacher ID is provided, check existence
            if (dto.ClassTeacherId.HasValue && !await _teacherRepository.ExistsAsync(dto.ClassTeacherId.Value, cancellationToken))
            {
                throw new NotFoundException($"Teacher with ID {dto.ClassTeacherId} not found.");
            }

            // Check whether the class has same academic year and class name exists
            if (await _classRepository.ExistsAsync(dto.Name, dto.AcademicYear, cancellationToken))
                throw new BusinessRuleViolationException($"Class '{dto.Name}' already exists for {dto.AcademicYear}.");

            // Map dto into model
            var newClassModel = dto.ToClassEntity();

            await _classRepository.CreateClassAsync(newClassModel, cancellationToken);

            var responseDto = newClassModel.ToClassDto();

            return responseDto;
        }


        public async Task DeleteClassAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existingClass = await _classRepository.GetClassByIdAsync(id, cancellationToken);

            if (existingClass == null)
                throw new NotFoundException($"Class with ID {id} not found.");

            if (existingClass.Students != null && existingClass.Students.Any())
                throw new BusinessRuleViolationException("Cannot delete class with enrolled students.");

            await _classRepository.DeleteClassAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync(CancellationToken cancellationToken = default)
        {
            
            var classes = await _classRepository.GetAllClassesAsync(cancellationToken);


            return classes.Select(c => c.ToClassDto());
        }

        public async Task<ClassDetailResponseDto> GetClassByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {

            var classModel = await _classRepository.GetClassByIdAsync(id, cancellationToken);
       
            if (classModel == null)
                throw new NotFoundException($"Class with ID {id} was not found.");
            var dto = classModel.ToClassDetailResponseDto();

            // Manually fetch usernames for each student
            foreach (var studentDto in dto.Students)
            {
                var user = await _userManager.FindByIdAsync(studentDto.IdentityUserId);
                studentDto.StudentName = user?.UserName ?? "Unknown User";
            }

            return dto;
        }

        public async Task<ClassResponseDto> UpdateClassAsync(Guid id, ClassUpdateDto dto, CancellationToken cancellationToken = default)
        {
   
            var existingClass = await _classRepository.GetClassByIdAsync(id, cancellationToken);
            if (existingClass == null)
                throw new NotFoundException($"Class with ID {id} not found.");


            if (existingClass.Name != dto.Name || existingClass.AcademicYear != dto.AcademicYear)
            {
                if (await _classRepository.ExistsAsync(dto.Name, dto.AcademicYear, cancellationToken))
                    throw new BusinessRuleViolationException("Another class already has this name for the chosen year.");
            }


            if (dto.ClassTeacherId.HasValue && dto.ClassTeacherId != existingClass.ClassTeacherId)
            {
                if (!await _teacherRepository.ExistsAsync(dto.ClassTeacherId.Value, cancellationToken))
                    throw new NotFoundException($"Teacher with ID {dto.ClassTeacherId} not found.");
            }


            existingClass.Name = dto.Name;
            existingClass.AcademicYear = dto.AcademicYear;
            existingClass.ClassTeacherId = dto.ClassTeacherId;
            existingClass.UpdatedAt = DateTime.UtcNow;

            
            await _classRepository.UpdateClassAsync(id, existingClass, cancellationToken);

            return existingClass.ToClassDto();
        }
    }
}
