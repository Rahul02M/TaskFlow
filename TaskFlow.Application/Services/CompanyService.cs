using TaskFlow.Application.DTOs.Companies;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public CompanyService(
            ICompanyRepository companyRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var currentUser =
                await _userRepository.GetByIdAsync(
                    _currentUserService.UserId);

            if (currentUser == null)
            {
                throw new UnauthorizedException(
                    "Current user was not found.");
            }

            if (currentUser.SystemRole != Domain.Enums.SystemRole.Admin &&
                currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to view companies.");
            }

            List<Company> companies;

            if (currentUser.SystemRole == Domain.Enums.SystemRole.SuperAdmin)
            {
                companies = await _companyRepository.GetAllAsync();
            }
            else
            {
                if (currentUser.CompanyId == null)
                {
                    throw new ForbiddenException(
                        "You are not assigned to a company.");
                }

                var company =
                    await _companyRepository.GetByIdForUserAsync(
                        currentUser.CompanyId,
                        _currentUserService.UserId);

                companies = company == null
                    ? new List<Company>()
                    : new List<Company> { company };
            }

            return companies.Select(company => new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt
            }).ToList();
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var currentUser =
                await _userRepository.GetByIdAsync(
                    _currentUserService.UserId);

            if (currentUser == null)
            {
                throw new UnauthorizedException(
                    "Current user was not found.");
            }

            if (currentUser.SystemRole != Domain.Enums.SystemRole.Admin &&
                currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to view companies.");
            }

            var company =
                await _companyRepository.GetByIdForUserAsync(
                    id,
                    _currentUserService.UserId);

            if (company == null)
                return null;

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task<CompanyDto> CreateAsync(
     CreateCompanyRequest request)
        {
            // 1. Get current logged-in user
            var currentUser =
                await _userRepository.GetByIdAsync(
                    _currentUserService.UserId);

            if (currentUser == null)
            {
                throw new UnauthorizedException(
                    "Current user was not found.");
            }

            // 2. Only SuperAdmin can create companies
            if (currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "Only SuperAdmin can create companies.");
            }

            // 3. Check duplicate company name
            var existingCompany =
                await _companyRepository.GetByNameAsync(
                    request.Name);

            if (existingCompany != null)
            {
                throw new ConflictException(
                    "Company name is already registered.");
            }

            // 4. Create company
            var company = new Company
            {
                Name = request.Name.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            // 5. Save
            await _companyRepository.AddAsync(company);

            // 6. Return DTO
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CreatedAt = company.CreatedAt
            };
        }

     
    public async Task UpdateAsync(int id, UpdateCompanyRequest request)
        {
            var currentUser = await _userRepository.GetByIdAsync(
                _currentUserService.UserId);

            if (currentUser == null)
            {
                throw new UnauthorizedException(
                    "Current user was not found.");
            }

            if (currentUser.SystemRole != Domain.Enums.SystemRole.Admin &&
                currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to update companies.");
            }

            var existingCompany =
                await _companyRepository.GetByIdForUserAsync(
                    id,
                    _currentUserService.UserId);

            if (existingCompany == null)
            {
                throw new NotFoundException(
                    $"Company with ID {id} was not found.");
            }

            var normalizedName = request.Name.Trim();

            var duplicateCompany =
                await _companyRepository.GetByNameAsync(normalizedName);

            if (duplicateCompany != null && duplicateCompany.Id != id)
            {
                throw new ConflictException(
                    "Company name is already registered.");
            }

            existingCompany.Name = normalizedName;
            existingCompany.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(existingCompany);
        }



        public async Task DeleteAsync(int id)
        {
            // 1. Get current logged-in user
            var currentUser =
                await _userRepository.GetByIdAsync(
                    _currentUserService.UserId);

            if (currentUser == null)
            {
                throw new UnauthorizedException(
                    "Current user was not found.");
            }

            // 2. Only Admin and SuperAdmin can delete companies
            if (currentUser.SystemRole != Domain.Enums.SystemRole.Admin &&
                currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to delete companies.");
            }

            // 3. Get company within current user's allowed scope
            var company =
                await _companyRepository.GetByIdForUserAsync(
                    id,
                    _currentUserService.UserId);

            if (company == null)
            {
                throw new NotFoundException(
                    $"Company with ID {id} was not found.");
            }

            // 4. Soft delete
            await _companyRepository.DeleteAsync(company);
        }
    }
}