using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userId, out var id))
                    throw new UnauthorizedAccessException("Invalid user identity.");

                return id;
            }
        }

        public SystemRole SystemRole
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.Role);

                if (!Enum.TryParse<SystemRole>(
                        role,
                        out var systemRole))
                {
                    throw new UnauthorizedAccessException(
                        "Invalid user role.");
                }

                return systemRole;
            }
        }
    }
}