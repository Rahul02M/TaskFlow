using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _projectRepository.GetAllAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task<Project> CreateAsync(CreateProjectRequest request)
        {

            var project = new Project
            {
                TeamId = request.TeamId,
                Name = request.Name,
                Description = request.Description,
                ProjectStatus = (ProjectStatus)request.ProjectStatus,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _projectRepository.AddAsync(project);

            return project;
        }


        public async Task<bool> UpdateAsync(Project project)
        {
            var existingProject = await _projectRepository.GetByIdAsync(project.Id);

            if (existingProject == null)
            {
                return false;
            }

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            existingProject.ProjectStatus = project.ProjectStatus;
            existingProject.UpdatedAt = DateTime.Now;

            await _projectRepository.UpdateAsync(existingProject);

            return true;

        }


        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project== null)
            {
                return false;
            }
            await _projectRepository.DeleteAsync(project);

            return true;
            
   
        }

    }


}
