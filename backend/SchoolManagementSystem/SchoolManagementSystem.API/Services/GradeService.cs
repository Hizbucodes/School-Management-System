using Microsoft.AspNetCore.Identity;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<GradeService> _logger;

        public GradeService(
            IGradeRepository gradeRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            UserManager<IdentityUser> userManager,
            ILogger<GradeService> logger)
        {
            _gradeRepository = gradeRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<(bool Succeeded, string Message, Guid? GradeId)> CreateGradeAsync(
            GradeCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate student exists
                if (!await _studentRepository.ExistsAsync(dto.StudentId, cancellationToken))
                {
                    return (false, "Student not found.", null);
                }

                // Validate course exists
                if (!await _courseRepository.ExistsAsync(dto.CourseId, cancellationToken))
                {
                    return (false, "Course not found.", null);
                }

                // Check if grade already exists for this student and course
                if (await _gradeRepository.ExistsForStudentCourseAsync(dto.StudentId, dto.CourseId, cancellationToken))
                {
                    return (false, "Grade already exists for this student and course.", null);
                }

                var grade = new Grade
                {
                    Id = Guid.NewGuid(),
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    Score = dto.Score,
                    GradeLetter = CalculateGradeLetter(dto.Score)
                };

                var created = await _gradeRepository.CreateAsync(grade, cancellationToken);

                return (true, "Grade created successfully.", created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating grade");
                return (false, "An error occurred while creating the grade.", null);
            }
        }

        public async Task<(bool Succeeded, string Message, int GradesCreated)> CreateBulkGradesAsync(
            BulkGradeCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate course exists
                if (!await _courseRepository.ExistsAsync(dto.CourseId, cancellationToken))
                {
                    return (false, "Course not found.", 0);
                }

                // Validate all students exist
                foreach (var studentGrade in dto.Grades)
                {
                    if (!await _studentRepository.ExistsAsync(studentGrade.StudentId, cancellationToken))
                    {
                        return (false, $"Student with ID {studentGrade.StudentId} not found.", 0);
                    }
                }

                // Check for existing grades
                var existingGrades = new List<Guid>();
                foreach (var studentGrade in dto.Grades)
                {
                    if (await _gradeRepository.ExistsForStudentCourseAsync(studentGrade.StudentId, dto.CourseId, cancellationToken))
                    {
                        existingGrades.Add(studentGrade.StudentId);
                    }
                }

                if (existingGrades.Any())
                {
                    return (false, $"Grades already exist for {existingGrades.Count} student(s).", 0);
                }

                // Create grades
                var grades = dto.Grades.Select(sg => new Grade
                {
                    Id = Guid.NewGuid(),
                    StudentId = sg.StudentId,
                    CourseId = dto.CourseId,
                    Score = sg.Score,
                    GradeLetter = CalculateGradeLetter(sg.Score)
                }).ToList();

                await _gradeRepository.CreateMultipleAsync(grades, cancellationToken);

                return (true, $"{grades.Count} grade(s) created successfully.", grades.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk grades");
                return (false, "An error occurred while creating grades.", 0);
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateGradeAsync(
            Guid id,
            GradeUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var grade = await _gradeRepository.GetByIdAsync(id, cancellationToken);
                if (grade == null)
                {
                    return (false, "Grade not found.");
                }

                grade.Score = dto.Score;
                grade.GradeLetter = CalculateGradeLetter(dto.Score);

                await _gradeRepository.UpdateAsync(grade, cancellationToken);

                return (true, "Grade updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating grade {GradeId}", id);
                return (false, "An error occurred while updating the grade.");
            }
        }

        public async Task<(bool Succeeded, string Message)> DeleteGradeAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _gradeRepository.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    return (false, "Grade not found.");
                }

                return (true, "Grade deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting grade {GradeId}", id);
                return (false, "An error occurred while deleting the grade.");
            }
        }

        public async Task<GradeResponseDto?> GetGradeByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var grade = await _gradeRepository.GetByIdAsync(id, cancellationToken);
            if (grade == null)
                return null;

            return MapToResponseDto(grade);
        }

        public async Task<IEnumerable<GradeResponseDto>> GetAllGradesAsync(
            CancellationToken cancellationToken = default)
        {
            var grades = await _gradeRepository.GetAllAsync(cancellationToken);
            return grades.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<GradeResponseDto>> GetStudentGradesAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var grades = await _gradeRepository.GetByStudentIdAsync(studentId, cancellationToken);
            return grades.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<GradeResponseDto>> GetCourseGradesAsync(
            Guid courseId,
            CancellationToken cancellationToken = default)
        {
            var grades = await _gradeRepository.GetByCourseIdAsync(courseId, cancellationToken);
            return grades.Select(MapToResponseDto);
        }

        public async Task<StudentReportCardDto?> GetStudentReportCardAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return null;

            var user = await _userManager.FindByIdAsync(student.IdentityUserId);
            var grades = await _gradeRepository.GetStudentGradesWithCoursesAsync(studentId, cancellationToken);
            var gradeList = grades.ToList();

            var courseGrades = gradeList.Select(g => new CourseGradeDto
            {
                CourseName = g.Course?.Name ?? "N/A",
                CourseCode = g.Course?.Code ?? "N/A",
                CreditHours = g.Course?.CreditHours ?? 0,
                Score = g.Score,
                GradeLetter = g.GradeLetter
            }).ToList();

            var gpa = gradeList.Any()
                ? Math.Round(gradeList.Average(g => ConvertGradeToGPA(g.GradeLetter)), 2)
                : 0;

            return new StudentReportCardDto
            {
                StudentId = studentId,
                AdmissionNumber = student.AdmissionNumber,
                Email = user?.Email ?? "N/A",
                ClassName = student.Class?.Name ?? "N/A",
                CourseGrades = courseGrades,
                OverallGPA = gpa,
                OverallGrade = CalculateGradeLetterFromGPA(gpa)
            };
        }

        public async Task<CourseGradeStatsDto?> GetCourseGradeStatsAsync(
            Guid courseId,
            CancellationToken cancellationToken = default)
        {
            var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
            if (course == null)
                return null;

            var grades = await _gradeRepository.GetByCourseIdAsync(courseId, cancellationToken);
            var gradeList = grades.ToList();

            if (!gradeList.Any())
            {
                return new CourseGradeStatsDto
                {
                    CourseId = courseId,
                    CourseName = course.Name,
                    CourseCode = course.Code,
                    TotalStudents = 0,
                    GradeDistribution = new GradeDistributionDto()
                };
            }

            var passedStudents = gradeList.Count(g => g.Score >= 50);
            var failedStudents = gradeList.Count - passedStudents;

            return new CourseGradeStatsDto
            {
                CourseId = courseId,
                CourseName = course.Name,
                CourseCode = course.Code,
                TotalStudents = gradeList.Count,
                AverageScore = Math.Round(gradeList.Average(g => g.Score), 2),
                HighestScore = gradeList.Max(g => g.Score),
                LowestScore = gradeList.Min(g => g.Score),
                PassedStudents = passedStudents,
                FailedStudents = failedStudents,
                PassPercentage = Math.Round((double)passedStudents / gradeList.Count * 100, 2),
                GradeDistribution = new GradeDistributionDto
                {
                    ACount = gradeList.Count(g => g.GradeLetter == "A"),
                    BCount = gradeList.Count(g => g.GradeLetter == "B"),
                    CCount = gradeList.Count(g => g.GradeLetter == "C"),
                    DCount = gradeList.Count(g => g.GradeLetter == "D"),
                    FCount = gradeList.Count(g => g.GradeLetter == "F")
                }
            };
        }

        private GradeResponseDto MapToResponseDto(Grade grade)
        {
            return new GradeResponseDto
            {
                Id = grade.Id,
                StudentId = grade.StudentId,
                StudentAdmissionNumber = grade.Student?.AdmissionNumber ?? "N/A",
                CourseId = grade.CourseId,
                CourseName = grade.Course?.Name ?? "N/A",
                CourseCode = grade.Course?.Code ?? "N/A",
                Score = grade.Score,
                GradeLetter = grade.GradeLetter
            };
        }

        private string CalculateGradeLetter(decimal score)
        {
            return score switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }

        private decimal ConvertGradeToGPA(string gradeLetter)
        {
            return gradeLetter switch
            {
                "A" => 4.0m,
                "B" => 3.0m,
                "C" => 2.0m,
                "D" => 1.0m,
                _ => 0.0m
            };
        }

        private string CalculateGradeLetterFromGPA(decimal gpa)
        {
            return gpa switch
            {
                >= 3.5m => "A",
                >= 2.5m => "B",
                >= 1.5m => "C",
                >= 0.5m => "D",
                _ => "F"
            };
        }
    }
}
