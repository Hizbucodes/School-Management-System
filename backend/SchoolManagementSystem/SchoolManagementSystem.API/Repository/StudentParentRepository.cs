using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class StudentParentRepository : IStudentParentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentParentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StudentParent?> GetRelationshipAsync(
            Guid studentId,
            Guid parentId,
            CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Include(sp => sp.Student)
                .Include(sp => sp.Parent)
                .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId, cancellationToken);
        }

        public async Task<IEnumerable<StudentParent>> GetByStudentIdAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Include(sp => sp.Parent)
                .Where(sp => sp.StudentId == studentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<StudentParent>> GetByParentIdAsync(
            Guid parentId,
            CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Include(sp => sp.Student)
                    .ThenInclude(s => s.Class)
                .Where(sp => sp.ParentId == parentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(
            Guid studentId,
            Guid parentId,
            CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .AnyAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId, cancellationToken);
        }

        public async Task<StudentParent> CreateAsync(
            StudentParent studentParent,
            CancellationToken cancellationToken = default)
        {
            await _context.StudentParents.AddAsync(studentParent, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return studentParent;
        }

        public async Task<bool> DeleteAsync(
            Guid studentId,
            Guid parentId,
            CancellationToken cancellationToken = default)
        {
            var studentParent = await _context.StudentParents
                .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId, cancellationToken);

            if (studentParent == null)
                return false;

            _context.StudentParents.Remove(studentParent);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task CreateMultipleAsync(
            IEnumerable<StudentParent> studentParents,
            CancellationToken cancellationToken = default)
        {
            await _context.StudentParents.AddRangeAsync(studentParents, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
