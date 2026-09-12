using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.Users;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var createdUser = await _userService.CreateAsync(request);

            return Ok(createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserRequest request)
        {
            var result = await _userService.UpdateAsync(id, request);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}