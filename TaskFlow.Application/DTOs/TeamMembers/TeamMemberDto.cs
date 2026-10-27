namespace TaskFlow.Application.DTOs.TeamMembers
{
    public class TeamMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int TeamRole { get; set; }
    }
}
