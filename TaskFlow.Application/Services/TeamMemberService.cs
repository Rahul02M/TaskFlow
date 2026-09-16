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
        public async Task<TeamMemberDto> CreateAsync(CreateTeamMemberRequest request)
        {
            // 1. Check user exists
            var userExists = await _teamMemberRepository.UserExistsAsync(request.UserId);
            if (!userExists)
                throw new InvalidOperationException("User does not exist or is inactive.");

            var exists = await _teamMemberRepository
            .ExistsAsync(request.UserId, request.TeamId);


            // 2. Check team exists
            var teamExists = await _teamMemberRepository.TeamExistsAsync(request.TeamId);

            if (!teamExists)
                throw new InvalidOperationException("Team does not exist.");

            // 3. Check duplicate membership
            var alreadyExists =
                await _teamMemberRepository.ExistsAsync(
                    request.UserId,
                    request.TeamId);

            if (alreadyExists)
                throw new InvalidOperationException( "User is already a member of this team.");

            // 4. Create entity
            var teamMember = new TeamMember
            {
                UserId = request.UserId,
                TeamId = request.TeamId,
                TeamRole = (TaskFlow.Domain.Enums.TeamRole)request.TeamRole,
                IsDeleted = false
            };
            // 5. Save
            await _teamMemberRepository.AddAsync(teamMember);

            // 6. Get created record with User + Team
            var createdTeamMember =
                await _teamMemberRepository.GetByIdAsync(teamMember.Id);

            if (createdTeamMember == null)
                throw new Exception(
                    "Created team member could not be found.");

            // 7. Return DTO
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


        public async Task<bool> UpdateAsync( int id, UpdateTeamMemberRequest request)
        {
            var existingTeamMember = await _teamMemberRepository.GetByIdAsync(id);

            if (existingTeamMember == null)
                return false;

            existingTeamMember.TeamRole = (TaskFlow.Domain.Enums.TeamRole)request.TeamRole;

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