using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Enums
{
    public enum TaskItemStatus
    {
        Pending,
        InProgress,
        Completed,
        OnHold,
        Cancelled
    }
}
