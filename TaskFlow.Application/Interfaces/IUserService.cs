using TaskFlow.Application.DTOs.Users;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        //Task<User> CreateAsync(User user);
        Task<User> CreateAsync(CreateUserRequest request);

        Task<bool> UpdateAsync(User user);

        Task<bool> DeleteAsync(int id);
    }
}