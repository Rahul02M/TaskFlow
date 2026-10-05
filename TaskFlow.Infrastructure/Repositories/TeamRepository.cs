using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly TaskFlowDbContext _context;

        public TeamRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Team>> GetAllAsync()
        {
            return await _context.Teams
                .Where(t =>
                    !t.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == t.CompanyId &&
                        !c.IsDeleted))
                .ToListAsync();
        }
        public async Task<List<Team>> GetAllByUserIdAsync(int userId)
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
                return new List<Team>();

            // SuperAdmin can access all active teams
            if (user.SystemRole == Domain.Enums.SystemRole.SuperAdmin)
            {
                return await _context.Teams
                 .Where(t =>
                     !t.IsDeleted &&
                     _context.Companies.Any(c =>
                         c.Id == t.CompanyId &&
                         !c.IsDeleted))
                 .ToListAsync();
             }

            // Admin can access teams in their own company
            if (user.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                return await _context.Teams
                .Where(t =>
                    !t.IsDeleted &&
                    t.CompanyId == user.CompanyId &&
                    _context.Companies.Any(c =>
                        c.Id == t.CompanyId &&
                        !c.IsDeleted))
                .ToListAsync();
            }

            // Normal User has no Team management access.
            return new List<Team>();
        }

        public async Task<Team?> GetByIdForUserAsync(int id, int userId)
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

            // SuperAdmin can access any active team
            if (user.SystemRole == Domain.Enums.SystemRole.SuperAdmin)
            {
                return await _context.Teams
                  .FirstOrDefaultAsync(t =>
                        t.Id == id &&
                        !t.IsDeleted &&
                        _context.Companies.Any(c =>
                            c.Id == t.CompanyId &&
                            !c.IsDeleted));
            }

            // Admin can access active teams in their own company
            if (user.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                    return await _context.Teams
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    !t.IsDeleted &&
                    t.CompanyId == user.CompanyId &&
                    _context.Companies.Any(c =>
                        c.Id == t.CompanyId &&
                        !c.IsDeleted));
            }

            // Normal User has no Team management access.
            return null;
        }
        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
        .FirstOrDefaultAsync(t =>
            t.Id == id &&
            !t.IsDeleted &&
            _context.Companies.Any(c =>
                c.Id == t.CompanyId &&
                !c.IsDeleted));
        }
        public async Task<Team?> GetByNameAsync( string name, int companyId)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(x =>
                    x.CompanyId == companyId &&
                    x.Name.ToLower() == name.ToLower() &&
                    !x.IsDeleted);
        }

        public async Task<bool> CompanyExistsAsync(int companyId)
        {
            return await _context.Companies
                .AnyAsync(x =>
                    x.Id == companyId &&
                    !x.IsDeleted);
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Team team)
        {
            team.IsDeleted = true;
            team.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

    }
}