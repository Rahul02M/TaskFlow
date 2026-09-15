using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.DTOs.Teams;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface ITeamService
    {

        Task<List<TeamDto>> GetAllAsync();
        Task<TeamDto?> GetByIdAsync(int id);
        Task<TeamDto> CreateAsync(CreateTeamRequest request);
        Task UpdateAsync(int id, UpdateTeamRequest request);
        Task DeleteAsync(int id);
    }
}
