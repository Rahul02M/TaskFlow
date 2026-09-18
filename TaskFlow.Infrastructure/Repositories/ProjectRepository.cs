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
            return await _context.Projects.Include(p => p.Team)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects.Include(p => p.Team)
                .FirstOrDefaultAsync(p =>
                    p.Id == id && !p.IsDeleted);
        }

        public async Task<Project?> GetByNameAsync(string name, int teamId)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Projects
                .FirstOrDefaultAsync(p =>
                    p.TeamId == teamId &&
                    p.Name.ToLower() == normalizedName &&
                    !p.IsDeleted);
        }

        public async Task<bool> TeamExistsAsync(int teamId)
        {
            return await _context.Teams
                .AnyAsync(t =>
                    t.Id == teamId &&
                    !t.IsDeleted);
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
                    !p.IsDeleted);
        }
    }
}