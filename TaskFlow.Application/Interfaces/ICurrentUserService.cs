using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        SystemRole SystemRole { get; }
    }
}
