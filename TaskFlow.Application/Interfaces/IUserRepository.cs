using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(int id);

        // Company-scoped queries for Admin
        Task<List<User>> GetAllByCompanyIdAsync(int companyId);
        Task<User?> GetByIdAndCompanyIdAsync(int id, int companyId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
       
       
    }
}