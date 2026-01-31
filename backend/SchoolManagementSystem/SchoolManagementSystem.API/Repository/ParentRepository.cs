using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class ParentRepository : IParentRepository
    {
        private readonly ApplicationDbContext _context;

        public ParentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Parent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Parents
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Parent?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Parents
                .FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId, cancellationToken);
        }

        public async Task<IEnumerable<Parent>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Parents
                .ToListAsync(cancellationToken);
        }

        public async Task<Parent> CreateAsync(Parent parent, CancellationToken cancellationToken = default)
        {
            await _context.Parents.AddAsync(parent, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return parent;
        }

        public async Task<Parent> UpdateAsync(Parent parent, CancellationToken cancellationToken = default)
        {
            _context.Parents.Update(parent);
            await _context.SaveChangesAsync(cancellationToken);
            return parent;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var parent = await _context.Parents.FindAsync(new object[] { id }, cancellationToken);
            if (parent == null)
                return false;

            _context.Parents.Remove(parent);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Parents.AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Parent?> GetParentWithStudentsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Parents
                .Include(p => p.StudentParents)
                    .ThenInclude(sp => sp.Student)
                        .ThenInclude(s => s.Class)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Parent>> GetParentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Parents
                .Where(p => p.StudentParents.Any(sp => sp.StudentId == studentId))
                .ToListAsync(cancellationToken);
        }
    }
}
