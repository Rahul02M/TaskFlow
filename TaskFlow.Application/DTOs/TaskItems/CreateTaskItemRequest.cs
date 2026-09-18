using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs.TaskItems;

public class CreateTaskItemRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    public int? AssignedUserId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 4)]
    public int TaskStatus { get; set; }

    [Range(0, 2)]
    public int TaskPriority { get; set; }

    public DateTime? DueDate { get; set; }
}