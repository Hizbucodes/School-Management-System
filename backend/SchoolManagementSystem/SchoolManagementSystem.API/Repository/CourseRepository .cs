using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.TeacherCourses)
                    .ThenInclude(tc => tc.Teacher)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
        }

        public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.TeacherCourses)
                    .ThenInclude(tc => tc.Teacher)
                .ToListAsync(cancellationToken);
        }

        public async Task<Course> CreateAsync(Course course, CancellationToken cancellationToken = default)
        {
            await _context.Courses.AddAsync(course, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return course;
        }

        public async Task<Course> UpdateAsync(Course course, CancellationToken cancellationToken = default)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync(cancellationToken);
            return course;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var course = await _context.Courses.FindAsync(new object[] { id }, cancellationToken);
            if (course == null)
                return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.AnyAsync(c => c.Code == code, cancellationToken);
        }

        public async Task<IEnumerable<Course>> GetCoursesByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.TeacherCourses)
                    .ThenInclude(tc => tc.Teacher)
                .Where(c => c.TeacherCourses.Any(tc => tc.TeacherId == teacherId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Course>> GetCoursesByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .Where(c => c.Enrollments.Any(e => e.StudentId == studentId))
                .ToListAsync(cancellationToken);
        }
    }
}
