using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Create(TaskItem taskItem)
        {
            var createdTask = await _taskItemService.CreateAsync(taskItem);

            return Ok(createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
                return BadRequest();

            var result = await _taskItemService.UpdateAsync(taskItem);

            if (!result)
                return NotFound();

            return NoContent();
        }

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