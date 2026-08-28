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
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<TeamMember?> GetByIdAsync(int id)
        {
            return await _context.TeamMembers
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
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
    }
}