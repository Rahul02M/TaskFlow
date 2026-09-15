using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TaskFlow.Application.DTOs.Companies
{
    public class CreateCompanyRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}
