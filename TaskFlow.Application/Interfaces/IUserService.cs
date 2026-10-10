using TaskFlow.Application.DTOs.Users;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface IUserService
    {
    
        Task<List<UserDto>> GetAllAsync(int currentUserId);
        Task<UserDto?> GetByIdAsync(int id, int currentUserId);
        //Task<User> CreateAsync(User user);
        //Task<UserDto> CreateAsync(CreateUserRequest request);
        Task<UserDto> CreateAsync(CreateUserRequest request, int currentUserId);
        Task<bool> UpdateAsync(int id, UpdateUserRequest request, int currentUserId);
        Task<bool> DeleteAsync(int id, int currentUserId);

    }
}