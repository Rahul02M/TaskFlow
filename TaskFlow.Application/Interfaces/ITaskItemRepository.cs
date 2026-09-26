using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<List<TaskItem>> GetAllAsync();

        Task<TaskItem?> GetByIdAsync(int id);

        Task<List<TaskItem>> GetAllForUserAsync(
            int userId,
            SystemRole systemRole,
            int? companyId);

        Task<TaskItem?> GetByIdForUserAsync(
            int id,
            int userId,
            SystemRole systemRole,
            int? companyId);

        Task AddAsync(TaskItem taskItem);

        Task UpdateAsync(TaskItem taskItem);

        Task DeleteAsync(TaskItem taskItem);
    }
}
