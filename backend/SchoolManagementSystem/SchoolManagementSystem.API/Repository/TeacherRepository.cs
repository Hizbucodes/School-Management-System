
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;

namespace SchoolManagementSystem.API.Repository
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext applicationDbContext)
        {
            this._context = applicationDbContext;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Teachers.AnyAsync(t => t.Id == id, cancellationToken);
        }
    }
}
