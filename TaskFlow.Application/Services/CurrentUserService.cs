using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskFlow.Application.Interfaces;

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

        public int SystemRole
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.Role);

                if (!int.TryParse(role, out var systemRole))
                    throw new UnauthorizedAccessException("Invalid user role.");

                return systemRole;
            }
        }
    }
}