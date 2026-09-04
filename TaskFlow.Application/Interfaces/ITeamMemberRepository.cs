using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface ITeamMemberRepository
    {
        Task<List<TeamMember>> GetAllAsync();

        Task<TeamMember?> GetByIdAsync(int id);

        Task AddAsync(TeamMember teamMember);

        Task UpdateAsync(TeamMember teamMember);

        Task DeleteAsync(TeamMember teamMember);

        Task<bool> ExistsAsync(int userId, int teamId);
    }
}