using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAllAsync();

        Task<Company?> GetByIdAsync(int id);

        Task<Company> CreateAsync(Company company);

        Task<bool> UpdateAsync(Company company);

        Task<bool> DeleteAsync(int id);
    }
}