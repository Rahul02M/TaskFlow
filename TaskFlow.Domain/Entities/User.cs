using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public SystemRole SystemRole { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }


}

