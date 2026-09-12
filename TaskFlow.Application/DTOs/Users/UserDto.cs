using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.Users
{
    public  class UserDto
    {
        public int Id { get; set; }

        public int? CompanyId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int SystemRole { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
