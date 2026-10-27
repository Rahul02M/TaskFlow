using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs.Projects
{
    public class CreateProjectRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int TeamId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 2)]
        public int ProjectStatus { get; set; }
    }
}
