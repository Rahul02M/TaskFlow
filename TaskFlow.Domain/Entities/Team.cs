

namespace TaskFlow.Domain.Entities
{
    public  class Team
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
