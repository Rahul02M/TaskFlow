using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TaskFlow.Application.DTOs.TaskItems
{
    public class UpdateTaskItemRequest
    {
        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }
        public int? AssignedUserId { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        [Range(0, 3)]
        public int TaskStatus { get; set; }
        [Range(0, 3)]
        public int TaskPriority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
