using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs.Teams;

public class UpdateTeamRequest
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}