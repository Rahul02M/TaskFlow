using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.TeamMembers
{
    public class CreateTeamMemberRequest
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public int TeamRole { get; set; }
    }
}
