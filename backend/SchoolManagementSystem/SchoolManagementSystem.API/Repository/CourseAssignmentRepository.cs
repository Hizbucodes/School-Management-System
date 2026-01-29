using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class CourseAssignmentRepository : ICourseAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseAssignmentRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<TeacherCourse?> GetAssignmentAsync(Guid courseId, Guid teacherId, CancellationToken ct)
        {
            return await _context.TeacherCourses.FirstOrDefaultAsync(tc => tc.CourseId == courseId && tc.TeacherId == teacherId, ct);
        }
            

        public async Task AddTeacherCourseAsync(TeacherCourse tc, CancellationToken ct)
        {
            await _context.TeacherCourses.AddAsync(tc, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddTeacherCoursesBatchAsync(IEnumerable<TeacherCourse> tcs, CancellationToken ct)
        {
            await _context.TeacherCourses.AddRangeAsync(tcs, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task RemoveTeacherCourseAsync(TeacherCourse tc, CancellationToken ct)
        {
            _context.TeacherCourses.Remove(tc);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Guid>> GetAssignedTeacherIdsAsync(Guid courseId, CancellationToken ct) =>
            await _context.TeacherCourses.Where(tc => tc.CourseId == courseId).Select(tc => tc.TeacherId).ToListAsync(ct);

        public async Task<Enrollment?> GetEnrollmentAsync(Guid courseId, Guid studentId, CancellationToken ct)
        {
            return await _context.Enrollments.FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId, ct);
        }
            

        public async Task AddEnrollmentAsync(Enrollment enrollment, CancellationToken ct)
        {
            await _context.Enrollments.AddAsync(enrollment, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddEnrollmentsBatchAsync(IEnumerable<Enrollment> enrollments, CancellationToken ct)
        {
            await _context.Enrollments.AddRangeAsync(enrollments, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task RemoveEnrollmentAsync(Enrollment enrollment, CancellationToken ct)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Guid>> GetEnrolledStudentIdsAsync(Guid courseId, CancellationToken ct)
        {
           return await _context.Enrollments.Where(e => e.CourseId == courseId).Select(e => e.StudentId).ToListAsync(ct);
        }
           
    }
}
