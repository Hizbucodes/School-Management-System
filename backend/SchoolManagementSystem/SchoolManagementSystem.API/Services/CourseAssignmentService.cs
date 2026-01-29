using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class CourseAssignmentService : ICourseAssignmentService
    {
        private readonly ICourseAssignmentRepository _assignmentRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly ITeacherRepository _teacherRepo;
        private readonly IStudentRepository _studentRepo;

        public CourseAssignmentService(ICourseAssignmentRepository courseAssignmentRepository,
            ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository)
        {
            _assignmentRepo = courseAssignmentRepository;
            _courseRepo = courseRepository;
            _teacherRepo = teacherRepository;
            _studentRepo = studentRepository;
        }

        public async Task<(bool Succeeded, string Message)> AssignTeacherToCourseAsync(Guid courseId, Guid teacherId, CancellationToken ct)
        {
            if (!await _courseRepo.ExistsAsync(courseId, ct)) return (false, "Course not found.");
            if (!await _teacherRepo.ExistsAsync(teacherId, ct)) return (false, "Teacher not found.");

            var existing = await _assignmentRepo.GetAssignmentAsync(courseId, teacherId, ct);
            if (existing != null) return (false, "Teacher is already assigned to this course.");

            await _assignmentRepo.AddTeacherCourseAsync(new TeacherCourse { CourseId = courseId, TeacherId = teacherId }, ct);
            return (true, "Teacher assigned successfully.");
        }

        public async Task<(bool Succeeded, string Message)> RemoveTeacherFromCourseAsync(Guid courseId, Guid teacherId, CancellationToken ct)
        {
            var assignment = await _assignmentRepo.GetAssignmentAsync(courseId, teacherId, ct);
            if (assignment == null) return (false, "Assignment not found.");

            await _assignmentRepo.RemoveTeacherCourseAsync(assignment, ct);
            return (true, "Teacher removed successfully.");
        }

        public async Task<(bool Succeeded, string Message)> AssignMultipleTeachersAsync(Guid courseId, List<Guid> teacherIds, CancellationToken ct)
        {
            if (!await _courseRepo.ExistsAsync(courseId, ct)) return (false, "Course not found.");

            var existingIds = await _assignmentRepo.GetAssignedTeacherIdsAsync(courseId, ct);
            var toAdd = teacherIds.Except(existingIds).Distinct().ToList();

            if (!toAdd.Any()) return (false, "All provided teachers are already assigned.");

            var newAssignments = toAdd.Select(id => new TeacherCourse { CourseId = courseId, TeacherId = id });
            await _assignmentRepo.AddTeacherCoursesBatchAsync(newAssignments, ct);

            return (true, $"{toAdd.Count} teacher(s) assigned successfully.");
        }





        public async Task<(bool Succeeded, string Message)> EnrollStudentAsync(Guid courseId, Guid studentId, string academicYear, CancellationToken ct)
        {
            if (!await _courseRepo.ExistsAsync(courseId, ct)) return (false, "Course not found.");
            if (!await _studentRepo.ExistsAsync(studentId, ct)) return (false, "Student not found.");

            var existing = await _assignmentRepo.GetEnrollmentAsync(courseId, studentId, ct);
            if (existing != null) return (false, "Student is already enrolled.");

            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                AcademicYear = academicYear,
            };

            await _assignmentRepo.AddEnrollmentAsync(enrollment, ct);
            return (true, "Student enrolled successfully.");
        }

        public async Task<(bool Succeeded, string Message)> UnenrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct)
        {
            var enrollment = await _assignmentRepo.GetEnrollmentAsync(courseId, studentId, ct);
            if (enrollment == null) return (false, "Enrollment not found.");

            await _assignmentRepo.RemoveEnrollmentAsync(enrollment, ct);
            return (true, "Student unenrolled successfully.");
        }

        public async Task<(bool Succeeded, string Message)> EnrollMultipleStudentsAsync(Guid courseId, List<Guid> studentIds, string academicYear, CancellationToken ct)
        {
            if (!await _courseRepo.ExistsAsync(courseId, ct)) return (false, "Course not found.");

            var existingIds = await _assignmentRepo.GetEnrolledStudentIdsAsync(courseId, ct);
            var toEnroll = studentIds.Except(existingIds).Distinct().ToList();

            if (!toEnroll.Any()) return (false, "All provided students are already enrolled.");

            var enrollments = toEnroll.Select(id => new Enrollment
            {
                CourseId = courseId,
                StudentId = id,
                AcademicYear = academicYear,
            });

            await _assignmentRepo.AddEnrollmentsBatchAsync(enrollments, ct);
            return (true, $"{toEnroll.Count} student(s) enrolled successfully.");
        }
    }

}
