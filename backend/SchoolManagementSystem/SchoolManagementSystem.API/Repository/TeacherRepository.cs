
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext applicationDbContext)
        {
            this._context = applicationDbContext;
        }

        public async Task<Teacher> CreateAsync(Teacher teacher, CancellationToken cancellationToken)
        {
            await _context.Teachers.AddAsync(teacher, cancellationToken);

            return teacher;
        }

        public async Task<Teacher?> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var teacherExists = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);

            if (teacherExists is null)
            {
                return null;
            }

            _context.Teachers.Remove(teacherExists);
            await _context.SaveChangesAsync(cancellationToken);
            return teacherExists;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Teachers.AnyAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Teachers.Include(t => t.IdentityUserId).AsNoTracking().ToListAsync();
        }

        public async Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<Teacher?> UpdateAsync(Teacher teacher, CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return teacher;
        }
    }
}
