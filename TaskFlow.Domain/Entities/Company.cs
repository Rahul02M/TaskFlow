using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TaskFlow.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
