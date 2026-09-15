using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.Teams;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamService.GetAllAsync();

            return Ok(teams);
        }

        // GET: api/Teams/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);

            if (team == null)
                return NotFound();

            return Ok(team);
        }

        // POST: api/Teams
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateTeamRequest request)
        {
            var team = await _teamService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = team.Id },
                team);
        }
        // PUT: api/Teams/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTeamRequest request)
        {
            await _teamService.UpdateAsync(id, request);

            return NoContent();
        }

        // DELETE: api/Teams/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _teamService.DeleteAsync(id);

            return NoContent();
        }
    }
}
