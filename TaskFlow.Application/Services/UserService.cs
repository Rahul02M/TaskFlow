using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;
            user.IsDeleted = false;

            await _userRepository.AddAsync(user);

            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);

            if (existingUser == null)
                return false;

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.CompanyId = user.CompanyId;
            existingUser.SystemRole = user.SystemRole;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(existingUser);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);

            return true;
        }
    }
}