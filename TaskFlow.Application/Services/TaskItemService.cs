using TaskFlow.Application.DTOs.TaskItems;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;


        public TaskItemService(ITaskItemRepository taskItemRepository, IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _taskItemRepository = taskItemRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        public async Task<List<TaskItemDto>> GetAllAsync()
        {
            var tasks = await _taskItemRepository.GetAllAsync();

            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TaskItemDto?> GetByIdAsync(int id)
        {
            var task = await _taskItemRepository.GetByIdAsync(id);

            if (task == null)
                return null;

            return MapToDto(task);
           
        }

        public async Task<TaskItemDto> CreateAsync(CreateTaskItemRequest request)
        {
            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);

            if (!projectExists)
            {
                throw new NotFoundException($"Project with ID {request.ProjectId} was not found.");
            }

            if (request.AssignedUserId.HasValue)
            {
                var userExists =
                    await _userRepository.ExistsAsync(request.AssignedUserId.Value);

                if (!userExists)
                    throw new NotFoundException($"User with ID {request.AssignedUserId.Value} was not found.");
            }

            if (!Enum.IsDefined(typeof(TaskItemStatus), request.TaskStatus))
            {
                throw new InvalidOperationException("Invalid task status.");
            }

            if (!Enum.IsDefined(typeof(TaskPriority), request.TaskPriority))
            {
                throw new InvalidOperationException("Invalid task priority.");
            }

            var taskItem = new TaskItem
            {
                ProjectId = request.ProjectId,
                AssignedUserId = request.AssignedUserId,
                Title = request.Title,
                Description = request.Description,
                TaskStatus = (TaskItemStatus)request.TaskStatus,
                TaskPriority = (TaskPriority)request.TaskPriority,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _taskItemRepository.AddAsync(taskItem);

            var createdTask = await _taskItemRepository.GetByIdAsync(taskItem.Id);

            if (createdTask == null)
                throw new InvalidOperationException(
                    "Task could not be retrieved after creation.");
            return MapToDto(createdTask);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTaskItemRequest request)
        {
            var existingTask = await _taskItemRepository.GetByIdAsync(id);

            if (existingTask == null)
                return false;
            if (!await _projectRepository.ExistsAsync(request.ProjectId))
            {
                throw new NotFoundException(
                    $"Project with ID {request.ProjectId} was not found.");
            }
            if (request.AssignedUserId.HasValue)
            {
                var userExists = await _userRepository.ExistsAsync(request.AssignedUserId.Value);

                if (!userExists)
                {
                    throw new NotFoundException(
                        $"User with ID {request.AssignedUserId.Value} was not found.");
                }
            }
            if (!Enum.IsDefined(typeof(TaskItemStatus),request.TaskStatus))
            {
                throw new InvalidOperationException("Invalid task status.");
            }

            if (!Enum.IsDefined(typeof(TaskPriority),request.TaskPriority))
            {
                throw new InvalidOperationException("Invalid task priority.");
            }

            existingTask.ProjectId = request.ProjectId;
            existingTask.AssignedUserId = request.AssignedUserId;
            existingTask.Title = request.Title.Trim();
            existingTask.Description =request.Description?.Trim() ?? string.Empty;

            existingTask.TaskStatus = (TaskItemStatus)request.TaskStatus;
            existingTask.TaskPriority = (TaskPriority)request.TaskPriority;

            existingTask.DueDate = request.DueDate;
            existingTask.CompletedAt = request.CompletedAt;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _taskItemRepository.UpdateAsync(existingTask);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var taskItem = await _taskItemRepository.GetByIdAsync(id);

            if (taskItem == null)
                return false;

            await _taskItemRepository.DeleteAsync(taskItem);

            return true;
        }

        private TaskItemDto MapToDto(TaskItem task)
        {
            return new TaskItemDto
            {
                Id = task.Id,

                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name ?? string.Empty,

                AssignedUserId = task.AssignedUserId,
                AssignedUserName = task.AssignedUser?.Name,

                Title = task.Title,
                Description = task.Description,

                TaskStatus = (int)task.TaskStatus,
                TaskPriority = (int)task.TaskPriority,

                DueDate = task.DueDate,
                CompletedAt = task.CompletedAt,

                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }

}