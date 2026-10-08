using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TaskFlow.Application.DTOs.TeamMembers
{
    public  class UpdateTeamMemberRequest
    {
        [Range(0, 1)]
        public int TeamRole { get; set; }
    }
}
