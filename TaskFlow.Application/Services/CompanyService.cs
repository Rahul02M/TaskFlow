using TaskFlow.Application.DTOs.Companies;
using TaskFlow.Application.Exceptions;
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

        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var companies = await _companyRepository.GetAllAsync();

            return companies.Select(company => new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt
            }).ToList();
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                return null;
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyRequest request)
        {
            var existingCompany = await _companyRepository.GetByNameAsync(request.Name);

            if (existingCompany != null)
            {
                throw new ConflictException("Company name is already registered.");
            }

            var company = new Company
            {
                Name = request.Name.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _companyRepository.AddAsync(company);

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task UpdateAsync(int id, UpdateCompanyRequest request)
        {
            var existingCompany =
                await _companyRepository.GetByIdAsync(id);

            if (existingCompany == null)
            {
                throw new NotFoundException(
                    $"Company with ID {id} was not found.");
            }

            var duplicateCompany =
                await _companyRepository.GetByNameAsync(request.Name);

            if (duplicateCompany != null &&
                duplicateCompany.Id != id)
            {
                throw new ConflictException(
                    "Company name is already registered.");
            }

            existingCompany.Name = request.Name.Trim();
            existingCompany.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(existingCompany);
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
            {
                throw new NotFoundException( $"Company with ID {id} was not found.");
            }

            await _companyRepository.DeleteAsync(company);
        }
    }
}