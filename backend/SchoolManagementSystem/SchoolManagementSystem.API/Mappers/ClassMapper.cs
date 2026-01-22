using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Mappers
{
    public static class ClassMapper
    {
        public static ClassResponseDto ToClassDto(this Class classModel)
        {
            return new ClassResponseDto
            {
                Id = classModel.Id,
                Name = classModel.Name,
                AcademicYear = classModel.AcademicYear,
                ClassTeacherId = classModel.ClassTeacherId,
                CreatedAt = classModel.CreatedAt
            };
        }

        public static ClassDetailResponseDto ToClassDetailResponseDto(this Class classModel)
        {
            return new ClassDetailResponseDto
            {
                Id = classModel.Id,
                Name = classModel.Name,
                AcademicYear = classModel.AcademicYear,
                ClassTeacherId = classModel.ClassTeacherId,
                ClassTeacher = classModel.ClassTeacher != null ? new TeacherDto
                {
                    Id = classModel.ClassTeacher.Id,
                    IdentityUserId = classModel.ClassTeacher.IdentityUserId,
                    Specialization = classModel.ClassTeacher.Specialization,
                } : null,
                Students = classModel.Students?.Select(s => new StudentDto
                {
                    Id = s.Id,
                    EnrollmentDate = s.EnrollmentDate,
                    AdmissionNumber = s.AdmissionNumber,
                    IdentityUserId = s.IdentityUserId,
    
                }).ToList() ?? new List<StudentDto>(),
                StudentCount = classModel.Students?.Count ?? 0,
                CreatedAt = classModel.CreatedAt,
                UpdatedAt = classModel.UpdatedAt,

            };
        }


        public static Class ToClassEntity(this ClassCreateDto dto)
        {
            return new Class
            {
                Name = dto.Name,
                AcademicYear = dto.AcademicYear,
                ClassTeacherId = dto.ClassTeacherId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
