using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.DTOs.Users;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<List<UserDto>> GetAllAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser == null || !currentUser.IsActive)
                throw new UnauthorizedException(
                    "Current user was not found or is inactive.");

            List<User> users;

            if (currentUser.SystemRole == SystemRole.SuperAdmin)
            {
                users = await _userRepository.GetAllAsync();
            }
            else if (currentUser.SystemRole == SystemRole.Admin)
            {
                if (currentUser.CompanyId == null)
                    throw new ForbiddenException(
                        "Your account is not assigned to a company.");

                users = await _userRepository.GetAllByCompanyIdAsync(
                    currentUser.CompanyId);
            }
            else
            {
                throw new ForbiddenException(
                    "You do not have permission to view users.");
            }

            return users.Select(user => new UserDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                Name = user.Name,
                Email = user.Email,
                SystemRole = (int)user.SystemRole,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            }).ToList();
        }
        public async Task<UserDto?> GetByIdAsync(int id, int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser == null || !currentUser.IsActive)
                throw new UnauthorizedException(
                    "Current user was not found or is inactive.");

            User? user;

            if (currentUser.SystemRole == SystemRole.SuperAdmin)
            {
                user = await _userRepository.GetByIdAsync(id);
            }
            else if (currentUser.SystemRole == SystemRole.Admin)
            {
                if (currentUser.CompanyId == null)
                    throw new ForbiddenException(
                        "Your account is not assigned to a company.");

                user = await _userRepository.GetByIdAndCompanyIdAsync(
                    id, currentUser.CompanyId);

                if (user == null)
                    throw new NotFoundException(
                        $"User with ID {id} was not found.");
            }
            else
            {
                throw new ForbiddenException(
                    "You do not have permission to view users.");
            }

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                Name = user.Name,
                Email = user.Email,
                SystemRole = (int)user.SystemRole,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
        public async Task<UserDto> CreateAsync(CreateUserRequest request,int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser == null || !currentUser.IsActive)
                throw new UnauthorizedException("Current user was not found or is inactive.");
            if (!Enum.IsDefined(typeof(SystemRole), request.SystemRole))
            {
                throw new BadRequestException("Invalid system role.");
            }

            var requestedRole = (SystemRole)request.SystemRole;

            if (currentUser.SystemRole != SystemRole.Admin &&
                currentUser.SystemRole != SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to create users.");
            }

            if (currentUser.SystemRole == SystemRole.Admin)
            {
                if (currentUser.CompanyId == null)
                    throw new ForbiddenException(
                        "Your account is not assigned to a company.");

                if (request.CompanyId.HasValue &&
                    request.CompanyId.Value != currentUser.CompanyId)
                {
                    throw new ForbiddenException(
                        "You cannot create users in another company.");
                }

                if (requestedRole != SystemRole.User)
                {
                    throw new ForbiddenException(
                        "Admin can only create normal users.");
                }
            }

            if (currentUser.SystemRole == SystemRole.SuperAdmin &&
                requestedRole == SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "SuperAdmin cannot create another SuperAdmin.");
            }

            int companyIdToAssign;

            if (currentUser.SystemRole == SystemRole.Admin)
            {
                companyIdToAssign = currentUser.CompanyId;
            }
            else
            {
                if (!request.CompanyId.HasValue)
                {
                    throw new BadRequestException(
                        "CompanyId is required when creating a user.");
                }

                companyIdToAssign = request.CompanyId.Value;
            }

            var company = await _companyRepository
                .GetByIdAsync(companyIdToAssign);

            if (company == null)
            {
                throw new NotFoundException(
                    $"Company with ID {companyIdToAssign} was not found.");
            }
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);

            if (existingUser != null)
                throw new ConflictException("Email is already registered.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                CompanyId = companyIdToAssign,
                SystemRole = requestedRole,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.AddAsync(user);

            return new UserDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                Name = user.Name,
                Email = user.Email,
                SystemRole = (int)user.SystemRole,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
        public async Task<bool> UpdateAsync(int id,UpdateUserRequest request,int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser == null || !currentUser.IsActive)
                throw new UnauthorizedException("Current user was not found or is inactive.");

            if (currentUser.SystemRole != SystemRole.Admin &&
                currentUser.SystemRole != SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to update users.");
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            // Admins can update only normal users in their own company.
            if (currentUser.SystemRole == SystemRole.Admin)
            {
                if (currentUser.CompanyId == null ||
                    user.CompanyId != currentUser.CompanyId)
                {
                    throw new ForbiddenException(
                        "You cannot update a user from another company.");
                }

                if (user.SystemRole != SystemRole.User)
                {
                    throw new ForbiddenException(
                        "Admin can only update normal users.");
                }

                if (request.SystemRole != (int)SystemRole.User)
                {
                    throw new ForbiddenException(
                        "Admin cannot change a user's system role.");
                }

                if (request.CompanyId.HasValue &&
                    request.CompanyId.Value != currentUser.CompanyId)
                {
                    throw new ForbiddenException(
                        "You cannot move a user to another company.");
                }
            }

            // Validate the requested role before casting it.
            if (!Enum.IsDefined(typeof(SystemRole), request.SystemRole))
            {
                throw new BadRequestException("Invalid system role.");
            }
            var requestedRole = (SystemRole)request.SystemRole;

            // SuperAdmins cannot change another SuperAdmin's role.
            if (currentUser.SystemRole == SystemRole.SuperAdmin &&
                user.SystemRole == SystemRole.SuperAdmin &&
                currentUser.Id != user.Id &&
                requestedRole != SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You cannot change another SuperAdmin's role.");
            }

            // Prevent promoting another user to SuperAdmin.
            if (currentUser.SystemRole == SystemRole.SuperAdmin &&
                currentUser.Id != user.Id &&
                requestedRole == SystemRole.SuperAdmin &&
                user.SystemRole != SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You cannot promote another user to SuperAdmin.");
            }
            // Avoid duplicate email addresses.
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);

            if (existingUser != null && existingUser.Id != user.Id)
                throw new ConflictException("Email is already registered.");

            user.Name = request.Name;
            user.Email = request.Email;
            user.SystemRole = requestedRole;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Admins cannot change company assignment.
            // SuperAdmins may change it when a CompanyId is supplied.

            if (currentUser.SystemRole == SystemRole.SuperAdmin &&
                request.CompanyId.HasValue)
            {
                var company = await _companyRepository
                    .GetByIdAsync(request.CompanyId.Value);

                if (company == null)
                {
                    throw new NotFoundException(
                        $"Company with ID {request.CompanyId.Value} was not found.");
                }

                user.CompanyId = company.Id;
            }
            await _userRepository.UpdateAsync(user);

            return true;
        }
        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser == null || !currentUser.IsActive)
                throw new UnauthorizedException(
                    "Current user was not found or is inactive.");

            if (currentUser.SystemRole != SystemRole.Admin &&
                currentUser.SystemRole != SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to delete users.");
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            if (currentUser.SystemRole == SystemRole.Admin)
            {
                if (currentUser.CompanyId == null ||
                    user.CompanyId != currentUser.CompanyId)
                {
                    throw new ForbiddenException(
                        "You cannot delete a user from another company.");
                }

                if (user.SystemRole != SystemRole.User)
                {
                    throw new ForbiddenException(
                        "Admin can only delete normal users.");
                }
            }

            // Prevent a user account from deleting itself.
            if (user.Id == currentUserId)
            {
                throw new ForbiddenException(
                    "You cannot delete your own account.");
            }

            user.IsDeleted = true;
            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}
