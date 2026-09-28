using TaskFlow.Application.DTOs.Teams;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;


        public TeamService(ITeamRepository teamRepository, ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<List<TeamDto>> GetAllAsync()
        {
            var teams = await _teamRepository
                .GetAllByUserIdAsync(_currentUserService.UserId);

            return teams.Select(team => new TeamDto
            {
                Id = team.Id,
                CompanyId = team.CompanyId,
                Name = team.Name,
                CreatedAt = team.CreatedAt
            }).ToList();
        }


        public async Task<TeamDto?> GetByIdAsync(int id)
        {
            var team = await _teamRepository
                .GetByIdForUserAsync(id, _currentUserService.UserId);

            if (team == null)
                return null;

            return new TeamDto
            {
                Id = team.Id,
                CompanyId = team.CompanyId,
                Name = team.Name,
                CreatedAt = team.CreatedAt
            };
        }
        public async Task<TeamDto> CreateAsync(CreateTeamRequest request)
        {
            // 1. Get current user
            var currentUser = await _userRepository.GetByIdAsync(
                _currentUserService.UserId);

            if (currentUser == null)
            {
                throw new UnauthorizedException(
                    "Current user was not found.");
            }

            // 2. Admin can create teams only in their own company
            if (currentUser.SystemRole == Domain.Enums.SystemRole.Admin &&
                currentUser.CompanyId != request.CompanyId)
            {
                throw new ForbiddenException(
                    "You do not have access to this company.");
            }

            // 3. Check Company
            var companyExists =
                await _teamRepository.CompanyExistsAsync(
                    request.CompanyId);

            if (!companyExists)
            {
                throw new NotFoundException(
                    $"Company with ID {request.CompanyId} was not found.");
            }

            // 4. Check duplicate team inside same company
            var existingTeam =
                await _teamRepository.GetByNameAsync(
                    request.Name,
                    request.CompanyId);

            if (existingTeam != null)
            {
                throw new ConflictException(
                    "Team name already exists in this company.");
            }

            // 5. Create entity
            var team = new Team
            {
                CompanyId = request.CompanyId,
                Name = request.Name.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _teamRepository.AddAsync(team);

            // 6. Return DTO
            return new TeamDto
            {
                Id = team.Id,
                CompanyId = team.CompanyId,
                Name = team.Name,
                CreatedAt = team.CreatedAt
            };
        }


        public async Task UpdateAsync(int id, UpdateTeamRequest request)
        {
            // 1. Get current user
            var currentUser = await _userRepository.GetByIdAsync(
                _currentUserService.UserId);

            if (currentUser == null)
            {
                throw new UnauthorizedException(
                    "Current user was not found.");
            }

            // 2. Get existing team
            var existingTeam =
                await _teamRepository.GetByIdAsync(id);

            if (existingTeam == null)
            {
                throw new NotFoundException(
                    $"Team with ID {id} was not found.");
            }

            // 3. Admin can update teams only in their own company
            if (currentUser.SystemRole == Domain.Enums.SystemRole.Admin &&
                currentUser.CompanyId != existingTeam.CompanyId)
            {
                throw new ForbiddenException(
                    "You do not have access to this team.");
            }

            // 4. Check duplicate team name inside same company
            var duplicateTeam =
                await _teamRepository.GetByNameAsync(
                    request.Name,
                    existingTeam.CompanyId);

            if (duplicateTeam != null &&
                duplicateTeam.Id != id)
            {
                throw new ConflictException(
                    "Team name already exists in this company.");
            }

            // 5. Update team
            existingTeam.Name = request.Name.Trim();
            existingTeam.UpdatedAt = DateTime.UtcNow;

            await _teamRepository.UpdateAsync(existingTeam);
        }

        public async Task DeleteAsync(int id)
        {
            var team =
                await _teamRepository.GetByIdAsync(id);

            if (team == null)
            {
                throw new NotFoundException(
                    $"Team with ID {id} was not found.");
            }

            await _teamRepository.DeleteAsync(team);
        }
    }
}