using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly TaskFlowDbContext _context;

        public TaskItemRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _context.TaskItems
              .Include(x => x.Project)
              .Include(x => x.AssignedUser)
              .Where(x => !x.IsDeleted && !x.Project.IsDeleted)
              .ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.TaskItems
                .Include(x => x.Project)
                .Include(x => x.AssignedUser)
                //.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                .FirstOrDefaultAsync(x =>x.Id == id && !x.IsDeleted && !x.Project.IsDeleted);
        }
        public async Task<List<TaskItem>> GetAllByUserIdAsync(int userId)
        {
            return await _context.TaskItems
                .Include(x => x.Project)
                    .ThenInclude(x => x.Team)
                        .ThenInclude(x => x!.TeamMembers)
                .Include(x => x.AssignedUser)
                .Where(x =>
                    !x.IsDeleted &&
                    !x.Project.IsDeleted &&
                    x.Project.Team != null &&
                    x.Project.Team.TeamMembers.Any(tm =>
                        tm.UserId == userId &&
                        !tm.IsDeleted))
                .ToListAsync();
        }
        public async Task<TaskItem?> GetByIdForUserAsync(int id, int userId)
        {
            return await _context.TaskItems
                .Include(x => x.Project)
                    .ThenInclude(x => x.Team)
                        .ThenInclude(x => x!.TeamMembers)
                .Include(x => x.AssignedUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted &&
                    !x.Project.IsDeleted &&
                    x.Project.Team != null &&
                    x.Project.Team.TeamMembers.Any(tm =>
                        tm.UserId == userId &&
                        !tm.IsDeleted));
        }

        public async Task AddAsync(TaskItem taskItem)
        {
            await _context.TaskItems.AddAsync(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem taskItem)
        {
            taskItem.IsDeleted = true;
            taskItem.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}