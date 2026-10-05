using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
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
                    .ThenInclude(x => x.Team)
                .Include(x => x.AssignedUser)
                .Where(x =>
                    !x.IsDeleted &&
                    !x.Project.IsDeleted &&
                    x.Project.Team != null &&
                    !x.Project.Team.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == x.Project.Team.CompanyId &&
                        !c.IsDeleted))
                .ToListAsync();
        }
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.TaskItems
                .Include(x => x.Project)
                    .ThenInclude(x => x.Team)
                .Include(x => x.AssignedUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted &&
                    !x.Project.IsDeleted &&
                    x.Project.Team != null &&
                    !x.Project.Team.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == x.Project.Team.CompanyId &&
                        !c.IsDeleted));
        }
        public async Task<List<TaskItem>> GetAllForUserAsync(int userId,SystemRole systemRole, int? companyId)
        {
            var query = _context.TaskItems
                .Include(x => x.Project)
                    .ThenInclude(x => x.Team)
                        .ThenInclude(x => x!.TeamMembers)
                .Include(x => x.AssignedUser)
                .Where(x =>
                    !x.IsDeleted &&
                    !x.Project.IsDeleted &&
                    x.Project.Team != null &&
                    !x.Project.Team.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == x.Project.Team.CompanyId &&
                        !c.IsDeleted));

            // SuperAdmin can access all tasks
            if (systemRole == SystemRole.SuperAdmin)
            {
                return await query.ToListAsync();
            }

            // Admin can access tasks inside their own company
            if (systemRole == SystemRole.Admin)
            {
                if (!companyId.HasValue)
                    return new List<TaskItem>();

                query = query.Where(x =>
                    x.Project.Team!.CompanyId == companyId.Value);
            }
            // Normal User can access tasks from their team memberships
            else
            {
                query = query.Where(x =>
                    x.Project.Team!.TeamMembers.Any(tm =>
                        tm.UserId == userId &&
                        !tm.IsDeleted));
            }

            return await query.ToListAsync();
        }
        public async Task<TaskItem?> GetByIdForUserAsync(int id,int userId,SystemRole systemRole,int? companyId)
        {
            var query = _context.TaskItems
                .Include(x => x.Project)
                    .ThenInclude(x => x.Team)
                        .ThenInclude(x => x!.TeamMembers)
                .Include(x => x.AssignedUser)
                .Where(x =>
                    x.Id == id &&
                    !x.IsDeleted &&
                    !x.Project.IsDeleted &&
                    x.Project.Team != null &&
                    !x.Project.Team.IsDeleted &&
                    _context.Companies.Any(c =>
                        c.Id == x.Project.Team.CompanyId &&
                        !c.IsDeleted));

            // SuperAdmin can access all tasks
            if (systemRole == SystemRole.SuperAdmin)
            {
                return await query.FirstOrDefaultAsync();
            }

            // Admin can access tasks inside their own company
            if (systemRole == SystemRole.Admin)
            {
                if (!companyId.HasValue)
                    return null;

                query = query.Where(x =>
                    x.Project.Team!.CompanyId == companyId.Value);
            }
            // Normal User can access tasks from their team memberships
            else
            {
                query = query.Where(x =>
                    x.Project.Team!.TeamMembers.Any(tm =>
                        tm.UserId == userId &&
                        !tm.IsDeleted));
            }

            return await query.FirstOrDefaultAsync();
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