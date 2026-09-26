using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly ICurrentUserService _currentUserService;

        public ProjectService(IProjectRepository projectRepository,ITeamMemberRepository teamMemberRepository,ICurrentUserService currentUserService)
        {
            _projectRepository = projectRepository;
            _teamMemberRepository = teamMemberRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ProjectDto>> GetAllAsync()
        {
            var projects = await _projectRepository.GetAllByUserIdAsync(_currentUserService.UserId);

            return projects.Select(MapToDto).ToList();
        }
        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdForUserAsync(id,_currentUserService.UserId);

            if (project == null)
                return null;

            return MapToDto(project);
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectRequest request)
        {
            if (!Enum.IsDefined(typeof(ProjectStatus),request.ProjectStatus))
            {
                throw new InvalidOperationException("Invalid project status.");
            }

            if (!await _projectRepository.TeamExistsAsync(request.TeamId))
            {
                throw new NotFoundException($"Team with ID {request.TeamId} was not found.");
            }
            if (!await HasTeamAccessAsync(request.TeamId))
            {
                throw new ForbiddenException("You do not have access to this team.");
            }
            var existingProject = await _projectRepository.GetByNameAsync(request.Name,request.TeamId);

            if (existingProject != null)
            {
                throw new ConflictException("Project name already exists in this team.");
            }

            var project = new Project
            {
                TeamId = request.TeamId,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                ProjectStatus = (ProjectStatus)request.ProjectStatus,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _projectRepository.AddAsync(project);

            var createdProject =
                await _projectRepository.GetByIdAsync(project.Id);

            if (createdProject == null)
            {
                throw new Exception(
                    "Created project could not be found.");
            }

            return MapToDto(createdProject);
        }

        public async Task<bool> UpdateAsync(int id,UpdateProjectRequest request)
        {
            if (!Enum.IsDefined(typeof(ProjectStatus),request.ProjectStatus))
            {
                throw new InvalidOperationException(
                    "Invalid project status.");
            }

            var existingProject =
                await _projectRepository.GetByIdAsync(id);

            if (existingProject == null)
                return false;
            if (!await HasTeamAccessAsync(existingProject.TeamId))
            {
                throw new ForbiddenException(
                    "You do not have access to this project.");
            }
            var duplicateProject =
                await _projectRepository.GetByNameAsync(
                    request.Name,
                    existingProject.TeamId);

            if (duplicateProject != null &&
                duplicateProject.Id != id)
            {
                throw new ConflictException(
                    "Project name already exists in this team.");
            }

            existingProject.Name = request.Name.Trim();

            existingProject.Description =
                request.Description?.Trim() ?? string.Empty;

            existingProject.ProjectStatus =
                (ProjectStatus)request.ProjectStatus;

            existingProject.UpdatedAt =
                DateTime.UtcNow;

            await _projectRepository.UpdateAsync(existingProject);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project =
                await _projectRepository.GetByIdAsync(id);

            if (project == null)
                return false;

            if (!await HasTeamAccessAsync(project.TeamId))
            {
                throw new ForbiddenException(
                    "You do not have access to this project.");
            }
            await _projectRepository.DeleteAsync(project);

            return true;
        }
        private async Task<bool> HasTeamAccessAsync(int teamId)
        {
            var team = await _teamMemberRepository
                .GetTeamWithCompanyAsync(teamId);

            if (team == null)
                return false;

            var user = await _teamMemberRepository
                .GetUserAsync(_currentUserService.UserId);

            if (user == null)
                return false;

            // SuperAdmin can access every team
            if (user.SystemRole == SystemRole.SuperAdmin)
                return true;

            // Admin can access teams inside their own company
            if (user.SystemRole == SystemRole.Admin)
                return user.CompanyId == team.CompanyId;

            // Normal User must be a member of the team
            return await _teamMemberRepository.ExistsAsync(
                _currentUserService.UserId,
                teamId);
        }
        private static ProjectDto MapToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                TeamId = project.TeamId,
                TeamName = project.Team?.Name ?? string.Empty,
                Name = project.Name,
                Description = project.Description,
                ProjectStatus = (int)project.ProjectStatus,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }
    }


}
