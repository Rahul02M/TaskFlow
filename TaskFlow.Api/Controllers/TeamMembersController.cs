using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.TeamMembers;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamMembersController : ControllerBase
    {
        private readonly ITeamMemberService _teamMemberService;

        public TeamMembersController(ITeamMemberService teamMemberService)
        {
            _teamMemberService = teamMemberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teamMembers = await _teamMemberService.GetAllAsync();

            return Ok(teamMembers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teamMember = await _teamMemberService.GetByIdAsync(id);

            if (teamMember == null)
                return NotFound();

            return Ok(teamMember);
        }

        [HttpPost]
        public async Task<IActionResult> Create( CreateTeamMemberRequest request)
        {
            try
            {
                var createdTeamMember = await _teamMemberService.CreateAsync(request);

                return Ok(createdTeamMember);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTeamMemberRequest request)
        {
            var result =
                await _teamMemberService.UpdateAsync(id, request);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _teamMemberService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}