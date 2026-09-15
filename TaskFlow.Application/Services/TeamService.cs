using TaskFlow.Application.DTOs.Teams;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<List<TeamDto>> GetAllAsync()
        {
            var teams = await _teamRepository.GetAllAsync();

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
            var team = await _teamRepository.GetByIdAsync(id);

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
        public async Task<TeamDto> CreateAsync( CreateTeamRequest request)
        {
            // 1. Check Company
            var companyExists =
                await _teamRepository.CompanyExistsAsync(
                    request.CompanyId);

            if (!companyExists)
            {
                throw new NotFoundException(
                    $"Company with ID {request.CompanyId} was not found.");
            }

            // 2. Check duplicate team inside same company
            var existingTeam =
                await _teamRepository.GetByNameAsync(
                    request.Name,
                    request.CompanyId);

            if (existingTeam != null)
            {
                throw new ConflictException(
                    "Team name already exists in this company.");
            }

            // 3. Create entity
            var team = new Team
            {
                CompanyId = request.CompanyId,
                Name = request.Name.Trim(),
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _teamRepository.AddAsync(team);

            // 4. Return DTO
            return new TeamDto
            {
                Id = team.Id,
                CompanyId = team.CompanyId,
                Name = team.Name,
                CreatedAt = team.CreatedAt
            };
        }


        public async Task UpdateAsync( int id, UpdateTeamRequest request)
        {
            var existingTeam =
                await _teamRepository.GetByIdAsync(id);

            if (existingTeam == null)
            {
                throw new NotFoundException(
                    $"Team with ID {id} was not found.");
            }

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