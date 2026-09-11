using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.TaskItems;
using TaskFlow.Application.Interfaces;


namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize]
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;

        public TaskItemsController(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskItemService.GetAllAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskItemService.GetByIdAsync(id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskItemRequest request)
        {
            var createdTask = await _taskItemService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTask.Id },
                createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTaskItemRequest request)
        {
            var result = await _taskItemService.UpdateAsync(id, request);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _taskItemService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}