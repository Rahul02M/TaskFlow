using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.Projects
{
    public class CreateProjectRequest
    {
        public int TeamId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int ProjectStatus { get; set; }
    }
}
