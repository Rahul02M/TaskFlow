using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.Projects
{
    public class ProjectDto
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int ProjectStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
