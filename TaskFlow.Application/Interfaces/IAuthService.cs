using TaskFlow.Application.DTOs.Auth;

namespace TaskFlow.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}