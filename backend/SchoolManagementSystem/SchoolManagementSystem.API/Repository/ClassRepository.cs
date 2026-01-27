using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<Class> CreateClassAsync(Class entity, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Class> DeleteClassAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var classExist = await _context.Classes.FirstOrDefaultAsync(c => c.Id == id);

            if (classExist is null)
            {
                return null;
            }

            _context.Classes.Remove(classExist);
           await _context.SaveChangesAsync(cancellationToken);
            return classExist;
        }

        public async Task<bool> ExistsAsync(string name, string year, CancellationToken cancellationToken = default)
        {
            return await _context.Classes.AnyAsync(c => c.Name == name && c.AcademicYear == year, cancellationToken);
        }

        public async Task<IEnumerable<Class>> GetAllClassesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Classes.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Class?> GetClassByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Classes.Include(c => c.ClassTeacher).Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Class> UpdateClassAsync(Guid id, Class entity, CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return entity;

        }
    }
}
