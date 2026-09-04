using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Application.DTOs.TeamMembers;

namespace TaskFlow.Application.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public TeamMemberService(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public async Task<List<TeamMemberDto>> GetAllAsync()
        {
            var teamMembers = await _teamMemberRepository.GetAllAsync();

            return teamMembers.Select(x => new TeamMemberDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User?.Name ?? string.Empty,
                TeamId = x.TeamId,
                TeamName = x.Team?.Name ?? string.Empty,
                TeamRole = (int)x.TeamRole
            }).ToList();
        }
        public async Task<TeamMemberDto?> GetByIdAsync(int id)
        {
            var teamMember = await _teamMemberRepository.GetByIdAsync(id);

            if (teamMember == null)
                return null;

            return new TeamMemberDto
            {
                Id = teamMember.Id,
                UserId = teamMember.UserId,
                UserName = teamMember.User?.Name ?? string.Empty,
                TeamId = teamMember.TeamId,
                TeamName = teamMember.Team?.Name ?? string.Empty,
                TeamRole = (int)teamMember.TeamRole
            };
        }
        public async Task<TeamMemberDto> CreateAsync(TeamMember teamMember)
        {
            var exists = await _teamMemberRepository
            .ExistsAsync(teamMember.UserId, teamMember.TeamId);

            if (exists)
                throw new InvalidOperationException(
                    "User is already a member of this team.");

            teamMember.IsDeleted = false;

            await _teamMemberRepository.AddAsync(teamMember);

            var createdTeamMember =
                await _teamMemberRepository.GetByIdAsync(teamMember.Id);

            if (createdTeamMember == null)
                throw new Exception("Created team member could not be found.");

            return new TeamMemberDto
            {
                Id = createdTeamMember.Id,
                UserId = createdTeamMember.UserId,
                UserName = createdTeamMember.User?.Name ?? string.Empty,
                TeamId = createdTeamMember.TeamId,
                TeamName = createdTeamMember.Team?.Name ?? string.Empty,
                TeamRole = (int)createdTeamMember.TeamRole
            };
        }

        public async Task<bool> UpdateAsync(TeamMember teamMember)
        {
            var existingTeamMember =
                await _teamMemberRepository.GetByIdAsync(teamMember.Id);

            if (existingTeamMember == null)
                return false;

            existingTeamMember.UserId = teamMember.UserId;
            existingTeamMember.TeamId = teamMember.TeamId;
            existingTeamMember.TeamRole = teamMember.TeamRole;

            await _teamMemberRepository.UpdateAsync(existingTeamMember);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teamMember = await _teamMemberRepository.GetByIdAsync(id);

            if (teamMember == null)
                return false;

            await _teamMemberRepository.DeleteAsync(teamMember);

            return true;
        }
    }
}