using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.Companies
{
    public  class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
