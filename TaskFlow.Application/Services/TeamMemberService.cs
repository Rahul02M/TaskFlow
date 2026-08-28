using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public TeamMemberService(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public async Task<List<TeamMember>> GetAllAsync()
        {
            return await _teamMemberRepository.GetAllAsync();
        }

        public async Task<TeamMember?> GetByIdAsync(int id)
        {
            return await _teamMemberRepository.GetByIdAsync(id);
        }

        public async Task<TeamMember> CreateAsync(TeamMember teamMember)
        {
            teamMember.IsDeleted = false;

            await _teamMemberRepository.AddAsync(teamMember);

            return teamMember;
        }

        public async Task<bool> UpdateAsync(TeamMember teamMember)
        {
            var existingTeamMember =
                await _teamMemberRepository.GetByIdAsync(teamMember.Id);

            if (existingTeamMember == null)
                return false;

            existingTeamMember.UserId = teamMember.UserId;
            existingTeamMember.TeamId = teamMember.TeamId;

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