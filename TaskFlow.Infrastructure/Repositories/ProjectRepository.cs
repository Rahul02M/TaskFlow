using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskFlowDbContext _context;

        public ProjectRepository(TaskFlowDbContext context)
        {
            _context = context;
        }


        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Team)
                .Where(p =>
                    !p.IsDeleted &&
                    p.Team != null &&
                    !p.Team.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == p.Team.CompanyId &&
                        !c.IsDeleted))
                .ToListAsync();
        }
        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    !p.IsDeleted &&
                    p.Team != null &&
                    !p.Team.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == p.Team.CompanyId &&
                        !c.IsDeleted));
        }
        public async Task<List<Project>> GetAllByUserIdAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId && !u.IsDeleted && u.IsActive)
                .Select(u => new
                {
                    u.CompanyId,
                    u.SystemRole
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return new List<Project>();

            // SuperAdmin can access all active projects
            if (user.SystemRole == Domain.Enums.SystemRole.SuperAdmin)
            {
                return await _context.Projects
                 .Include(p => p.Team)
                 .Where(p =>
                     !p.IsDeleted &&
                     p.Team != null &&
                     !p.Team.IsDeleted &&
                     _context.Companies.Any(c =>
                         c.Id == p.Team.CompanyId &&
                         !c.IsDeleted))
                 .ToListAsync();
            }

            // Admin can access all active projects in their company
            if (user.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                return await _context.Projects
                   .Include(p => p.Team)
                   .Where(p =>
                       !p.IsDeleted &&
                       p.Team != null &&
                       !p.Team.IsDeleted &&
                       p.Team.CompanyId == user.CompanyId &&
                       _context.Companies.Any(c =>
                           c.Id == p.Team.CompanyId &&
                           !c.IsDeleted))
                   .ToListAsync();
            }

            // Normal User can access only active projects
            // belonging to active teams they are an active member of
            return await _context.Projects
            .Include(p => p.Team)
            .Where(p =>
                !p.IsDeleted &&
                p.Team != null &&
                !p.Team.IsDeleted &&
                _context.Companies.Any(c =>
                    c.Id == p.Team.CompanyId &&
                    !c.IsDeleted) &&
                p.Team.TeamMembers.Any(tm =>
                    tm.UserId == userId &&
                    !tm.IsDeleted))
            .ToListAsync();
        }
        public async Task<Project?> GetByIdForUserAsync(int id, int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId && !u.IsDeleted && u.IsActive)
                .Select(u => new
                {
                    u.CompanyId,
                    u.SystemRole
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            // SuperAdmin can access all active projects
            if (user.SystemRole == Domain.Enums.SystemRole.SuperAdmin)
            {
                return await _context.Projects
                 .Include(p => p.Team)
                 .FirstOrDefaultAsync(p =>
                     p.Id == id &&
                     !p.IsDeleted &&
                     p.Team != null &&
                     !p.Team.IsDeleted &&
                     _context.Companies.Any(c =>
                         c.Id == p.Team.CompanyId &&
                         !c.IsDeleted));
            }

            // Admin can access active projects in their company
            if (user.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                return await _context.Projects
                    .Include(p => p.Team)
                    .FirstOrDefaultAsync(p =>
                        p.Id == id &&
                        !p.IsDeleted &&
                        p.Team != null &&
                        !p.Team.IsDeleted &&
                        p.Team.CompanyId == user.CompanyId &&
                        _context.Companies.Any(c =>
                            c.Id == p.Team.CompanyId &&
                            !c.IsDeleted));
            }

            // Normal User can access only active projects
            // belonging to active teams they are an active member of
            return await _context.Projects
                 .Include(p => p.Team)
                 .FirstOrDefaultAsync(p =>
                     p.Id == id &&
                     !p.IsDeleted &&
                     p.Team != null &&
                     !p.Team.IsDeleted &&
                     _context.Companies.Any(c =>
                         c.Id == p.Team.CompanyId &&
                         !c.IsDeleted) &&
                     p.Team.TeamMembers.Any(tm =>
                         tm.UserId == userId &&
                         !tm.IsDeleted));
        }
        public async Task<Project?> GetByNameAsync(string name, int teamId)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Projects
                .FirstOrDefaultAsync(p =>
                    p.TeamId == teamId &&
                    p.Name.ToLower() == normalizedName &&
                    !p.IsDeleted &&
                    _context.Teams.Any(t =>
                        t.Id == p.TeamId &&
                        !t.IsDeleted &&
                        _context.Companies.Any(c =>
                            c.Id == t.CompanyId &&
                            !c.IsDeleted)));
        }

        public async Task<bool> TeamExistsAsync(int teamId)
        {
            return await _context.Teams
                .AnyAsync(t =>
                    t.Id == teamId &&
                    !t.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == t.CompanyId &&
                        !c.IsDeleted));
        }
        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Project project)
        {
            project.IsDeleted = true;
            project.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Projects
                .AnyAsync(p =>
                    p.Id == id &&
                    !p.IsDeleted &&
                    _context.Teams.Any(t =>
                        t.Id == p.TeamId &&
                        !t.IsDeleted &&
                        _context.Companies.Any(c =>
                            c.Id == t.CompanyId &&
                            !c.IsDeleted)));
        }

        public async Task<int?> GetTeamIdAsync(int projectId)
        {
            return await _context.Projects
                .Where(p =>
                    p.Id == projectId &&
                    !p.IsDeleted &&
                    _context.Teams.Any(t =>
                        t.Id == p.TeamId &&
                        !t.IsDeleted &&
                        _context.Companies.Any(c =>
                            c.Id == t.CompanyId &&
                            !c.IsDeleted)))
                .Select(p => (int?)p.TeamId)
                .FirstOrDefaultAsync();
        }
    }
}