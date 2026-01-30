using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IClassRepository _classRepository;
        private readonly ILogger<ExamService> _logger;

        public ExamService(
            IExamRepository examRepository,
            ICourseRepository courseRepository,
            IClassRepository classRepository,
            ILogger<ExamService> logger)
        {
            _examRepository = examRepository;
            _courseRepository = courseRepository;
            _classRepository = classRepository;
            _logger = logger;
        }

        public async Task<(bool Succeeded, string Message, Guid? ExamId)> CreateExamAsync(
            ExamCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate course exists
                if (!await _courseRepository.ExistsAsync(dto.CourseId, cancellationToken))
                {
                    return (false, "Course not found.", null);
                }

                // Validate class exists
                if (!await _classRepository.ExistAsync(dto.ClassId, cancellationToken))
                {
                    return (false, "Class not found.", null);
                }

                // Validate time range
                if (dto.EndTime <= dto.StartTime)
                {
                    return (false, "End time must be after start time.", null);
                }

                // Check if exam already exists for this course on this date
                if (await _examRepository.ExistsForCourseOnDateAsync(dto.CourseId, dto.ExamDate, cancellationToken))
                {
                    return (false, "An exam for this course already exists on the selected date.", null);
                }

                var exam = new Exam
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    CourseId = dto.CourseId,
                    ClassId = dto.ClassId,
                    ExamDate = dto.ExamDate.Date,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    TotalMarks = dto.TotalMarks
                };

                var created = await _examRepository.CreateAsync(exam, cancellationToken);

                return (true, $"Exam '{created.Name}' created successfully.", created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                return (false, "An error occurred while creating the exam.", null);
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateExamAsync(
            Guid id,
            ExamUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(id, cancellationToken);
                if (exam == null)
                {
                    return (false, "Exam not found.");
                }

                // Validate time range
                if (dto.EndTime <= dto.StartTime)
                {
                    return (false, "End time must be after start time.");
                }

                exam.Name = dto.Name;
                exam.ExamDate = dto.ExamDate.Date;
                exam.StartTime = dto.StartTime;
                exam.EndTime = dto.EndTime;
                exam.TotalMarks = dto.TotalMarks;

                await _examRepository.UpdateAsync(exam, cancellationToken);

                return (true, $"Exam '{exam.Name}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam {ExamId}", id);
                return (false, "An error occurred while updating the exam.");
            }
        }

        public async Task<(bool Succeeded, string Message)> DeleteExamAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var exam = await _examRepository.GetExamWithStudentsAsync(id, cancellationToken);
                if (exam == null)
                {
                    return (false, "Exam not found.");
                }

                // Check if exam has student records
                if (exam.StudentExams.Any())
                {
                    return (false, "Cannot delete exam with existing student records.");
                }

                var deleted = await _examRepository.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    return (false, "Failed to delete the exam.");
                }

                return (true, "Exam deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam {ExamId}", id);
                return (false, "An error occurred while deleting the exam.");
            }
        }

        public async Task<ExamResponseDto?> GetExamByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var exam = await _examRepository.GetByIdAsync(id, cancellationToken);
            if (exam == null)
                return null;

            return MapToResponseDto(exam);
        }

        public async Task<ExamDetailDto?> GetExamDetailsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var exam = await _examRepository.GetExamWithStudentsAsync(id, cancellationToken);
            if (exam == null)
                return null;

            return new ExamDetailDto
            {
                Id = exam.Id,
                Name = exam.Name,
                CourseId = exam.CourseId,
                CourseName = exam.Course?.Name ?? "N/A",
                CourseCode = exam.Course?.Code ?? "N/A",
                ClassId = exam.ClassId,
                ClassName = exam.Class?.Name ?? "N/A",
                ExamDate = exam.ExamDate,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                TotalMarks = exam.TotalMarks,
                Duration = (int)(exam.EndTime - exam.StartTime).TotalMinutes,
                RegisteredStudents = exam.StudentExams.Select(se => new ExamStudentDto
                {
                    StudentId = se.StudentId,
                    AdmissionNumber = se.Student?.AdmissionNumber ?? "N/A",
                    MarksObtained = se.MarksObtained,
                    IsAbsent = se.IsAbsent
                }).ToList()
            };
        }

        public async Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync(
            CancellationToken cancellationToken = default)
        {
            var exams = await _examRepository.GetAllAsync(cancellationToken);
            return exams.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ExamResponseDto>> GetExamsByCourseAsync(
            Guid courseId,
            CancellationToken cancellationToken = default)
        {
            var exams = await _examRepository.GetByCourseIdAsync(courseId, cancellationToken);
            return exams.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ExamResponseDto>> GetExamsByClassAsync(
            Guid classId,
            CancellationToken cancellationToken = default)
        {
            var exams = await _examRepository.GetByClassIdAsync(classId, cancellationToken);
            return exams.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ExamResponseDto>> GetUpcomingExamsAsync(
            CancellationToken cancellationToken = default)
        {
            var exams = await _examRepository.GetUpcomingExamsAsync(cancellationToken);
            return exams.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ExamScheduleDto>> GetExamScheduleAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var exams = await _examRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);

            var schedule = exams
                .GroupBy(e => e.ExamDate.Date)
                .Select(g => new ExamScheduleDto
                {
                    Date = g.Key,
                    Exams = g.Select(MapToResponseDto).OrderBy(e => e.StartTime).ToList()
                })
                .OrderBy(s => s.Date)
                .ToList();

            return schedule;
        }

        private ExamResponseDto MapToResponseDto(Exam exam)
        {
            return new ExamResponseDto
            {
                Id = exam.Id,
                Name = exam.Name,
                CourseId = exam.CourseId,
                CourseName = exam.Course?.Name ?? "N/A",
                CourseCode = exam.Course?.Code ?? "N/A",
                ClassId = exam.ClassId,
                ClassName = exam.Class?.Name ?? "N/A",
                ExamDate = exam.ExamDate,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                TotalMarks = exam.TotalMarks,
                Duration = (int)(exam.EndTime - exam.StartTime).TotalMinutes,
                RegisteredStudents = exam.StudentExams?.Count ?? 0
            };
        }
    }
}
