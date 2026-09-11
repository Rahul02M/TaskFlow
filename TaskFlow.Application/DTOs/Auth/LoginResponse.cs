using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int SystemRole { get; set; }
    }
}
