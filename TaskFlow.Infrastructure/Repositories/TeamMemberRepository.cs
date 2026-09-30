using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TeamMemberRepository : ITeamMemberRepository
    {
        private readonly TaskFlowDbContext _context; 

        public TeamMemberRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeamMember>> GetAllAsync()
        {
            return await _context.TeamMembers
                .Include(x => x.User)
                .Include(x => x.Team)
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }
        public async Task<TeamMember?> GetByIdAsync(int id)
        {
            return await _context.TeamMembers
                .Include(x => x.User)
                .Include(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }
        public async Task<TeamMember?> GetByIdForUserAsync(int id,int userId)
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
                return await _context.TeamMembers
                    .Include(x => x.User)
                    .Include(x => x.Team)
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        !x.IsDeleted &&
                        !x.Team!.IsDeleted &&
                        !x.User!.IsDeleted &&
                        x.User.IsActive);
            }

            if (user.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                return await _context.TeamMembers
                    .Include(x => x.User)
                    .Include(x => x.Team)
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        !x.IsDeleted &&
                        !x.Team!.IsDeleted &&
                        !x.User!.IsDeleted &&
                        x.User.IsActive &&
                        x.Team.CompanyId == user.CompanyId);
            }

            return null;
        }
        public async Task<List<TeamMember>> GetAllByUserIdAsync(int userId)
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
                return new List<TeamMember>();

            if (user.SystemRole == Domain.Enums.SystemRole.SuperAdmin)
            {
                return await _context.TeamMembers
                    .Include(x => x.User)
                    .Include(x => x.Team)
                    .Where(x =>
                        !x.IsDeleted &&
                        !x.Team!.IsDeleted &&
                        !x.User!.IsDeleted &&
                        x.User.IsActive)
                    .ToListAsync();
            }

            if (user.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                return await _context.TeamMembers
                    .Include(x => x.User)
                    .Include(x => x.Team)
                    .Where(x =>
                        !x.IsDeleted &&
                        !x.Team!.IsDeleted &&
                        !x.User!.IsDeleted &&
                        x.User.IsActive &&
                        x.Team.CompanyId == user.CompanyId)
                    .ToListAsync();
            }

            return new List<TeamMember>();
        }
        public async Task AddAsync(TeamMember teamMember)
        {
            await _context.TeamMembers.AddAsync(teamMember);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(TeamMember teamMember)
        {
            _context.TeamMembers.Update(teamMember);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(TeamMember teamMember)
        {
            teamMember.IsDeleted = true;
            teamMember.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(int userId, int teamId)
        {
            return await _context.TeamMembers
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.TeamId == teamId &&
                    !x.IsDeleted);
        }
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(x =>
                    x.Id == userId &&
                    !x.IsDeleted &&
                    x.IsActive);
        }
        public async Task<bool> TeamExistsAsync(int teamId)
        {
            return await _context.Teams
                .AnyAsync(x =>
                    x.Id == teamId &&
                    !x.IsDeleted);
        }
        public async Task<TeamMember?> GetByUserIdAndTeamIdAsync(int userId, int teamId)
        {
            return await _context.TeamMembers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TeamId == teamId && !x.IsDeleted);
        }
        public async Task<Team?> GetTeamWithCompanyAsync(int teamId)
        {
            return await _context.Teams
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t =>
                    t.Id == teamId &&
                    !t.IsDeleted);
        }
        public async Task<User?> GetUserAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == userId &&
                    !u.IsDeleted &&
                    u.IsActive);
        }

    }
}