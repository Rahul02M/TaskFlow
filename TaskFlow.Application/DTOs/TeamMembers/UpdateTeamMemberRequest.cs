using System.ComponentModel.DataAnnotations;


namespace TaskFlow.Application.DTOs.TeamMembers
{
    public class UpdateTeamMemberRequest
    {
        [Range(0, 1)]
        public int TeamRole { get; set; }
    }
}
