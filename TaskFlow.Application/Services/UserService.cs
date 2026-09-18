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
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

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
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

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
        public async Task<UserDto> CreateAsync(CreateUserRequest request, string currentUserRole)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new ConflictException("Email is already registered.");
            }

            if (!Enum.IsDefined(typeof(SystemRole), request.SystemRole))
            {
                throw new InvalidOperationException("Invalid system role.");
            }
            var requestedRole = (SystemRole)request.SystemRole;

            if (currentUserRole == "Admin" &&
                requestedRole != SystemRole.User)
            {
                throw new InvalidOperationException(
                    "Admin can only create normal users.");
            }

            if (currentUserRole == "SuperAdmin" &&
                requestedRole == SystemRole.SuperAdmin)
            {
                throw new InvalidOperationException(
                    "SuperAdmin cannot create another SuperAdmin.");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                CompanyId = request.CompanyId,
                SystemRole = (SystemRole)request.SystemRole,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password);

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
        public async Task<bool> UpdateAsync( int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            user.Name = request.Name;
            user.Email = request.Email;
            user.CompanyId = request.CompanyId;
            user.SystemRole = (SystemRole)request.SystemRole;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            user.IsDeleted = true;
            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}
