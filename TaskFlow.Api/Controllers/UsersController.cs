using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.Users;
using TaskFlow.Application.Interfaces;
using System.Security.Claims;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
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
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var currentUserId))
                return Unauthorized();

            var users = await _userService.GetAllAsync(currentUserId);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var currentUserId))
                return Unauthorized();

            var user = await _userService.GetByIdAsync(id, currentUserId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateUserRequest request)
        //{
        //    var createdUser = await _userService.CreateAsync(request);

        //    return CreatedAtAction(
        //        nameof(GetById),
        //        new { id = createdUser.Id },
        //        createdUser);
        //}


        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var currentUserId))
                return Unauthorized();

            var createdUser = await _userService.CreateAsync(request, currentUserId);

            return Ok(createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserRequest request)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var currentUserId))
                return Unauthorized();

            var result = await _userService.UpdateAsync(id, request, currentUserId);

            if (!result)
                return NotFound();

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var currentUserId))
                return Unauthorized();

            await _userService.DeleteAsync(id, currentUserId);

            return NoContent();
        }
    }
}