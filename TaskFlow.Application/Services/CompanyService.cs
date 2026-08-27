using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _companyRepository.GetAllAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _companyRepository.GetByIdAsync(id);
        }

        public async Task<Company> CreateAsync(Company company)
        {
            company.CreatedAt = DateTime.UtcNow;
            company.IsDeleted = false;

            await _companyRepository.AddAsync(company);

            return company;
        }

        public async Task<bool> UpdateAsync(Company company)
        {
            var existingCompany = await _companyRepository.GetByIdAsync(company.Id);

            if (existingCompany == null)
                return false;

            existingCompany.Name = company.Name;
            existingCompany.UpdatedAt = DateTime.Now;

            await _companyRepository.UpdateAsync(existingCompany);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                return false;

            await _companyRepository.DeleteAsync(company);

            return true;
        }
    }
}