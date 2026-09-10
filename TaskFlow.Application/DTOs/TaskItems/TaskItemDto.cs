using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.TaskItems
{
    public  class TaskItemDto
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int TaskStatus { get; set; }

        public int TaskPriority { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
