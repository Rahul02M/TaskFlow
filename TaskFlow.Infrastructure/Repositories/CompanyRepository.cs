using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly TaskFlowDbContext _context;

        public CompanyRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }
        public async Task<Company?> GetByIdForUserAsync(int id,int userId)
        {
            var user = await _context.Users
                .Where(u =>
                    u.Id == userId &&
                    !u.IsDeleted &&
                    u.IsActive)
                .Select(u => new
                {
                    u.CompanyId,
                    u.SystemRole
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            if (user.SystemRole == Domain.Enums.SystemRole.SuperAdmin)
            {
                return await _context.Companies
                    .FirstOrDefaultAsync(c =>
                        c.Id == id &&
                        !c.IsDeleted);
            }

            if (user.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                return await _context.Companies
                    .FirstOrDefaultAsync(c =>
                        c.Id == id &&
                        !c.IsDeleted &&
                        c.Id == user.CompanyId);
            }

            return null;
        }
        
        public async Task<Company?> GetByNameAsync(string name)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower() &&
                    !x.IsDeleted);
        }
        public async Task AddAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Company company)
        {
            company.IsDeleted = true;
            company.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}