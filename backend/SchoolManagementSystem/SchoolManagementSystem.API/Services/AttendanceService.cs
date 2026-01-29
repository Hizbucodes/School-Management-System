using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            ICourseRepository courseRepository,
            ILogger<AttendanceService> logger)
        {
            _attendanceRepository = attendanceRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<(bool Succeeded, string Message, Guid? AttendanceId)> CreateAttendanceAsync(
            AttendanceCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate student exists
                if (!await _studentRepository.ExistsAsync(dto.StudentId, cancellationToken))
                {
                    return (false, "Student not found.", null);
                }

                // Validate class exists
                if (!await _classRepository.ExistAsync(dto.ClassId, cancellationToken))
                {
                    return (false, "Class not found.", null);
                }

                // Validate course exists
                if (!await _courseRepository.ExistsAsync(dto.CourseId, cancellationToken))
                {
                    return (false, "Course not found.", null);
                }

                // Check if attendance already exists for this student, course, and date
                if (await _attendanceRepository.AttendanceExistsAsync(dto.StudentId, dto.CourseId, dto.Date, cancellationToken))
                {
                    return (false, "Attendance record already exists for this student, course, and date.", null);
                }

                var attendance = new Attendance
                {
                    Id = Guid.NewGuid(),
                    StudentId = dto.StudentId,
                    ClassId = dto.ClassId,
                    CourseId = dto.CourseId,
                    Date = dto.Date.Date, // Store only date, not time
                    IsPresent = dto.IsPresent
                };

                var created = await _attendanceRepository.CreateAsync(attendance, cancellationToken);

                return (true, "Attendance marked successfully.", created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating attendance record");
                return (false, "An error occurred while marking attendance.", null);
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateAttendanceAsync(
            Guid id,
            AttendanceUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var attendance = await _attendanceRepository.GetByIdAsync(id, cancellationToken);
                if (attendance == null)
                {
                    return (false, "Attendance record not found.");
                }

                attendance.IsPresent = dto.IsPresent;

                await _attendanceRepository.UpdateAsync(attendance, cancellationToken);

                return (true, "Attendance updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating attendance {AttendanceId}", id);
                return (false, "An error occurred while updating attendance.");
            }
        }

        public async Task<(bool Succeeded, string Message)> DeleteAttendanceAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _attendanceRepository.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    return (false, "Attendance record not found.");
                }

                return (true, "Attendance record deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attendance {AttendanceId}", id);
                return (false, "An error occurred while deleting attendance.");
            }
        }

        public async Task<(bool Succeeded, string Message, int RecordsCreated)> MarkBulkAttendanceAsync(
            BulkAttendanceCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate class exists
                if (!await _classRepository.ExistAsync(dto.ClassId, cancellationToken))
                {
                    return (false, "Class not found.", 0);
                }

                // Validate course exists
                if (!await _courseRepository.ExistsAsync(dto.CourseId, cancellationToken))
                {
                    return (false, "Course not found.", 0);
                }

                // Validate all students exist
                foreach (var student in dto.Students)
                {
                    if (!await _studentRepository.ExistsAsync(student.StudentId, cancellationToken))
                    {
                        return (false, $"Student with ID {student.StudentId} not found.", 0);
                    }
                }

                // Check for existing attendance records
                var existingRecords = new List<Guid>();
                foreach (var student in dto.Students)
                {
                    if (await _attendanceRepository.AttendanceExistsAsync(student.StudentId, dto.CourseId, dto.Date, cancellationToken))
                    {
                        existingRecords.Add(student.StudentId);
                    }
                }

                if (existingRecords.Any())
                {
                    return (false, $"Attendance already exists for {existingRecords.Count} student(s) on this date.", 0);
                }

                // Create attendance records
                var attendances = dto.Students.Select(s => new Attendance
                {
                    Id = Guid.NewGuid(),
                    StudentId = s.StudentId,
                    ClassId = dto.ClassId,
                    CourseId = dto.CourseId,
                    Date = dto.Date.Date,
                    IsPresent = s.IsPresent
                }).ToList();

                await _attendanceRepository.CreateMultipleAsync(attendances, cancellationToken);

                return (true, $"Attendance marked for {attendances.Count} student(s).", attendances.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking bulk attendance");
                return (false, "An error occurred while marking attendance.", 0);
            }
        }

        public async Task<AttendanceResponseDto?> GetAttendanceByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id, cancellationToken);
            if (attendance == null)
                return null;

            return MapToResponseDto(attendance);
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetAllAttendanceAsync(
            CancellationToken cancellationToken = default)
        {
            var attendances = await _attendanceRepository.GetAllAsync(cancellationToken);
            return attendances.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetStudentAttendanceAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _attendanceRepository.GetByStudentIdAsync(studentId, cancellationToken);
            return attendances.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetClassAttendanceAsync(
            Guid classId,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _attendanceRepository.GetByClassIdAsync(classId, cancellationToken);
            return attendances.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetCourseAttendanceAsync(
            Guid courseId,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _attendanceRepository.GetByCourseIdAsync(courseId, cancellationToken);
            return attendances.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetAttendanceByDateAsync(
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _attendanceRepository.GetByDateAsync(date, cancellationToken);
            return attendances.Select(MapToResponseDto);
        }

        public async Task<AttendanceReportDto?> GetCourseAttendanceReportAsync(
            Guid courseId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
            if (course == null)
                return null;

            var attendances = await _attendanceRepository.GetByCourseAndDateAsync(courseId, date, cancellationToken);
            var attendanceList = attendances.ToList();

            var presentCount = attendanceList.Count(a => a.IsPresent);
            var totalCount = attendanceList.Count;

            return new AttendanceReportDto
            {
                CourseId = courseId,
                CourseName = course.Name,
                Date = date.Date,
                TotalStudents = totalCount,
                PresentStudents = presentCount,
                AbsentStudents = totalCount - presentCount,
                AttendancePercentage = totalCount > 0 ? Math.Round((double)presentCount / totalCount * 100, 2) : 0,
                Records = attendanceList.Select(MapToResponseDto).ToList()
            };
        }

        public async Task<AttendanceStatsDto?> GetStudentAttendanceStatsAsync(
            Guid studentId,
            Guid? courseId,
            CancellationToken cancellationToken = default)
        {
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return null;

            var attendances = await _attendanceRepository.GetByStudentIdAsync(studentId, cancellationToken);

            if (courseId.HasValue)
            {
                attendances = attendances.Where(a => a.CourseId == courseId.Value);
            }

            var attendanceList = attendances.ToList();
            var presentCount = attendanceList.Count(a => a.IsPresent);
            var totalCount = attendanceList.Count;

            return new AttendanceStatsDto
            {
                StudentId = studentId,
                StudentAdmissionNumber = student.AdmissionNumber,
                TotalClasses = totalCount,
                PresentCount = presentCount,
                AbsentCount = totalCount - presentCount,
                AttendancePercentage = totalCount > 0 ? Math.Round((double)presentCount / totalCount * 100, 2) : 0
            };
        }

        private AttendanceResponseDto MapToResponseDto(Attendance attendance)
        {
            return new AttendanceResponseDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                StudentAdmissionNumber = attendance.Student?.AdmissionNumber ?? "N/A",
                ClassId = attendance.ClassId,
                ClassName = attendance.Class?.Name ?? "N/A",
                CourseId = attendance.CourseId,
                CourseName = attendance.Course?.Name ?? "N/A",
                CourseCode = attendance.Course?.Code ?? "N/A",
                Date = attendance.Date,
                IsPresent = attendance.IsPresent
            };
        }
    }
}
