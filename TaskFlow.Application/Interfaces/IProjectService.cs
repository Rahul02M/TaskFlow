using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetAllAsync();
        Task<ProjectDto?> GetByIdAsync(int id);
        Task<ProjectDto> CreateAsync(CreateProjectRequest request);
        Task<bool> UpdateAsync(int id, UpdateProjectRequest request);
        Task<bool> DeleteAsync( int id);


    }
}
