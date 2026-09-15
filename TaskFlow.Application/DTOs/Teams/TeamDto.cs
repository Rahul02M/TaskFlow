using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.Teams
{
    public class TeamDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
