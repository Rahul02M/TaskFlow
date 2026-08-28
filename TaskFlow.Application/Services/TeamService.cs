using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<List<Team>> GetAllAsync()
        {
            return await _teamRepository.GetAllAsync();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _teamRepository.GetByIdAsync(id);
        }

        public async Task<Team> CreateAsync(Team team)
        {
            team.CreatedAt = DateTime.Now;
            team.IsDeleted = false;

            await _teamRepository.AddAsync(team);

            return team;
        }

        public async Task<bool> UpdateAsync(Team team)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(team.Id);

            if (existingTeam == null)
                return false;

            existingTeam.Name = team.Name;
            existingTeam.CompanyId = team.CompanyId;
            existingTeam.UpdatedAt = DateTime.Now;

            await _teamRepository.UpdateAsync(existingTeam);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);

            if (team == null)
                return false;

            await _teamRepository.DeleteAsync(team);

            return true;
        }
    }
}