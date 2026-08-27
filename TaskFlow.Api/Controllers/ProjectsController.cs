using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }


        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> Create (CreateProjectRequest request)
        {
             var project = await  _projectService.CreateAsync(request);

            return Ok(project);
        }

    }
}
