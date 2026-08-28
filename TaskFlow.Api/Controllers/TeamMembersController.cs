using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create(TeamMember teamMember)
        {
            var createdTeamMember =
                await _teamMemberService.CreateAsync(teamMember);

            return Ok(createdTeamMember);
        }
    }
}