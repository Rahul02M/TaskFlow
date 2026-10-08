using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TaskFlow.Application.DTOs.TeamMembers
{
    public class CreateTeamMemberRequest
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Range(1, int.MaxValue)]
        public int TeamId { get; set; }

        [Range(0, 1)]
        public int TeamRole { get; set; }
    }
}
