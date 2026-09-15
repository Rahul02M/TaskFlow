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
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }
        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted);
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
            team.DeletedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}