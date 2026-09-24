using TaskFlow.Application.DTOs.TaskItems;

namespace TaskFlow.Application.Interfaces
{
    public interface ITaskItemService
    {
        Task<List<TaskItemDto>> GetAllAsync();
        Task<TaskItemDto?> GetByIdAsync(int id);
        Task<TaskItemDto> CreateAsync(CreateTaskItemRequest request);
        Task<bool> UpdateAsync(int id, UpdateTaskItemRequest request);
        Task<bool> DeleteAsync(int id);
    }
}