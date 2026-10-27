using TaskFlow.Application.DTOs.Projects;

namespace TaskFlow.Application.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetAllAsync();
        Task<ProjectDto?> GetByIdAsync(int id);
        Task<ProjectDto> CreateAsync(CreateProjectRequest request);
        Task<bool> UpdateAsync(int id, UpdateProjectRequest request);
        Task<bool> DeleteAsync(int id);


    }
}
