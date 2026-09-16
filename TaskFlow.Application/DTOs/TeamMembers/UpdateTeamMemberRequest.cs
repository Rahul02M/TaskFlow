using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TaskFlow.Application.DTOs.TeamMembers
{
    public  class UpdateTeamMemberRequest
    {
        [Required]
        public int TeamRole { get; set; }
    }
}
