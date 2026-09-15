using TaskFlow.Application.DTOs.Companies;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllAsync();

        Task<CompanyDto?> GetByIdAsync(int id);

        Task<CompanyDto> CreateAsync(CreateCompanyRequest request);

        Task UpdateAsync(int id, UpdateCompanyRequest request);
        Task DeleteAsync(int id);
    }
}