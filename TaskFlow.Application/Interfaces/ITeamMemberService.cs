using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface ITeamMemberService
    {
        Task<List<TeamMember>> GetAllAsync();

        Task<TeamMember?> GetByIdAsync(int id);

        Task<TeamMember> CreateAsync(TeamMember teamMember);

        Task<bool> UpdateAsync(TeamMember teamMember);

        Task<bool> DeleteAsync(int id);
    }
}