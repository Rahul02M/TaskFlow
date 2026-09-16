using TaskFlow.Application.DTOs.TeamMembers;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface ITeamMemberService
    {
        Task<List<TeamMemberDto>> GetAllAsync();

        Task<TeamMemberDto?> GetByIdAsync(int id);

        Task<TeamMemberDto> CreateAsync(CreateTeamMemberRequest request);

        Task<bool> UpdateAsync(int id, UpdateTeamMemberRequest request);

        Task<bool> DeleteAsync(int id);
    }
}