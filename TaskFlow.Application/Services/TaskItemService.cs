using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public TaskItemService(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _taskItemRepository.GetAllAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _taskItemRepository.GetByIdAsync(id);
        }

        public async Task<TaskItem> CreateAsync(TaskItem taskItem)
        {
            taskItem.CreatedAt = DateTime.Now;
            taskItem.IsDeleted = false;

            await _taskItemRepository.AddAsync(taskItem);

            return taskItem;
        }

        public async Task<bool> UpdateAsync(TaskItem taskItem)
        {
            var existingTask =
                await _taskItemRepository.GetByIdAsync(taskItem.Id);

            if (existingTask == null)
                return false;

            existingTask.Title = taskItem.Title;
            existingTask.Description = taskItem.Description;
            existingTask.TaskStatus = taskItem.TaskStatus;
            existingTask.TaskPriority = taskItem.TaskPriority;
            existingTask.AssignedUserId = taskItem.AssignedUserId;
            existingTask.DueDate = taskItem.DueDate;
            existingTask.CompletedAt = taskItem.CompletedAt;
            existingTask.UpdatedAt = DateTime.Now;

            await _taskItemRepository.UpdateAsync(existingTask);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var taskItem =
                await _taskItemRepository.GetByIdAsync(id);

            if (taskItem == null)
                return false;

            await _taskItemRepository.DeleteAsync(taskItem);

            return true;
        }
    }
}