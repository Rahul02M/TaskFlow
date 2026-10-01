using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        SystemRole SystemRole { get; }
    }
}
