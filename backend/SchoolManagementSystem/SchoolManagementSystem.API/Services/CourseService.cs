using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ICourseRepository courseRepository, ILogger<CourseService> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }



        public async Task<(bool Succeeded, string Message, Guid? CourseId)> CreateCourseAsync(
            CourseCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if course code already exists
                if (await _courseRepository.ExistsByCodeAsync(dto.Code, cancellationToken))
                {
                    return (false, "Course code already exists.", null);
                }

                var course = new Course
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Code = dto.Code,
                    CreditHours = dto.CreditHours
                };

                var createdCourse = await _courseRepository.CreateAsync(course, cancellationToken);

                return (true, $"Course '{createdCourse.Name}' created successfully.", createdCourse.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course with code {Code}", dto.Code);
                return (false, "An error occurred while creating the course.", null);
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateCourseAsync(
            Guid id,
            CourseUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
                if (course == null)
                {
                    return (false, "Course not found.");
                }

                course.Name = dto.Name;
                course.CreditHours = dto.CreditHours;

                await _courseRepository.UpdateAsync(course, cancellationToken);

                return (true, $"Course '{course.Name}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating course {CourseId}", id);
                return (false, "An error occurred while updating the course.");
            }
        }

        public async Task<(bool Succeeded, string Message)> DeleteCourseAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
                if (course == null)
                {
                    return (false, "Course not found.");
                }

                // Check if course has enrollments
                if (course.Enrollments.Any())
                {
                    return (false, "Cannot delete course with active enrollments.");
                }

                var deleted = await _courseRepository.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    return (false, "Failed to delete the course.");
                }

                return (true, "Course deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course {CourseId}", id);
                return (false, "An error occurred while deleting the course.");
            }
        }

        public async Task<CourseResponseDto?> GetCourseByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
            if (course == null)
                return null;

            return new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                CreditHours = course.CreditHours,
                EnrolledStudentsCount = course.Enrollments.Count,
                AssignedTeachersCount = course.TeacherCourses.Count,
                AssignedTeachers = course.TeacherCourses.Select(tc => new AssignedTeacherDto
                {
                    TeacherId = tc.TeacherId,
                    FullName = tc.Teacher.FullName,
                    Specialization = tc.Teacher.Specialization
                }).ToList()
            };
        }

        public async Task<CourseDetailDto?> GetCourseDetailsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
            if (course == null)
                return null;

            return new CourseDetailDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                CreditHours = course.CreditHours,
                Teachers = course.TeacherCourses.Select(tc => new AssignedTeacherDto
                {
                    TeacherId = tc.TeacherId,
                    FullName = tc.Teacher.FullName,
                    Specialization = tc.Teacher.Specialization
                }).ToList(),
                Students = course.Enrollments.Select(e => new EnrolledStudentDto
                {
                    StudentId = e.StudentId,
                    AdmissionNumber = e.Student.AdmissionNumber,
                    AcademicYear = e.AcademicYear
                }).ToList()
            };
        }

        public async Task<IEnumerable<CourseResponseDto>> GetAllCoursesAsync(
            CancellationToken cancellationToken = default)
        {
            var courses = await _courseRepository.GetAllAsync(cancellationToken);

            return courses.Select(course => new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                CreditHours = course.CreditHours,
                EnrolledStudentsCount = course.Enrollments?.Count ?? 0,
                AssignedTeachersCount = course.TeacherCourses?.Count ?? 0,
                AssignedTeachers = course.TeacherCourses?.Select(tc => new AssignedTeacherDto
                {
                    TeacherId = tc.TeacherId,
                    FullName = tc.Teacher?.FullName ?? "N/A",
                    Specialization = tc.Teacher?.Specialization ?? "N/A"
                }).ToList() ?? new List<AssignedTeacherDto>()
            });
        }

        public async Task<IEnumerable<CourseResponseDto>> GetCoursesByTeacherAsync(
            Guid teacherId,
            CancellationToken cancellationToken = default)
        {
            var courses = await _courseRepository.GetCoursesByTeacherAsync(teacherId, cancellationToken);

            return courses.Select(course => new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                CreditHours = course.CreditHours,
                EnrolledStudentsCount = course.Enrollments?.Count ?? 0,
                AssignedTeachersCount = course.TeacherCourses?.Count ?? 0
            });
        }

        public async Task<IEnumerable<CourseResponseDto>> GetCoursesByStudentAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var courses = await _courseRepository.GetCoursesByStudentAsync(studentId, cancellationToken);

            return courses.Select(course => new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                CreditHours = course.CreditHours,
                EnrolledStudentsCount = course.Enrollments?.Count ?? 0,
                AssignedTeachersCount = course.TeacherCourses?.Count ?? 0
            });
        }
    }
}
