using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<Team>> GetAllAsync();

        Task<Team?> GetByIdAsync(int id);

        Task AddAsync(Team team);

        Task UpdateAsync(Team team);

        Task DeleteAsync(Team team);
    }
}