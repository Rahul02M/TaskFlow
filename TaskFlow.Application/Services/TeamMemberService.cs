using TaskFlow.Application.DTOs.TeamMembers;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        public TeamMemberService(ITeamMemberRepository teamMemberRepository,IUserRepository userRepository,ICurrentUserService currentUserService)
        {
            _teamMemberRepository = teamMemberRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<TeamMemberDto>> GetAllAsync()
        {
            var teamMembers =
                await _teamMemberRepository.GetAllByUserIdAsync(
                    _currentUserService.UserId);

            return teamMembers.Select(x => new TeamMemberDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User?.Name ?? string.Empty,
                TeamId = x.TeamId,
                TeamName = x.Team?.Name ?? string.Empty,
                TeamRole = (int)x.TeamRole
            }).ToList();
        }
        public async Task<TeamMemberDto?> GetByIdAsync(int id)
        {
            var teamMember = await _teamMemberRepository.GetByIdForUserAsync(id,_currentUserService.UserId);

            if (teamMember == null)
                return null;

            return new TeamMemberDto
            {
                Id = teamMember.Id,
                UserId = teamMember.UserId,
                UserName = teamMember.User?.Name ?? string.Empty,
                TeamId = teamMember.TeamId,
                TeamName = teamMember.Team?.Name ?? string.Empty,
                TeamRole = (int)teamMember.TeamRole
            };
        }
        public async Task<TeamMemberDto> CreateAsync(CreateTeamMemberRequest request)
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

            // 2. Only Admin and SuperAdmin can manage TeamMembers
            if (currentUser.SystemRole != Domain.Enums.SystemRole.Admin &&
                currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to manage team members.");
            }

            // 3. Get target user
            var targetUser =
                await _teamMemberRepository.GetUserAsync(
                    request.UserId);

            if (targetUser == null)
            {
                throw new NotFoundException(
                    $"User with ID {request.UserId} was not found.");
            }

            // 4. Get target team
            var team =
                await _teamMemberRepository.GetTeamWithCompanyAsync(
                    request.TeamId);

            if (team == null)
            {
                throw new NotFoundException(
                    $"Team with ID {request.TeamId} was not found.");
            }

            // 5. Admin can manage only teams in their own company
            if (currentUser.SystemRole == Domain.Enums.SystemRole.Admin)
            {
                if (currentUser.CompanyId != team.CompanyId)
                {
                    throw new ForbiddenException(
                        "You do not have access to this team.");
                }

                if (targetUser.CompanyId != team.CompanyId)
                {
                    throw new ForbiddenException(
                        "You cannot add a user from another company to this team.");
                }
            }

            // 6. User and Team must belong to the same company
            if (targetUser.CompanyId != team.CompanyId)
            {
                throw new ConflictException(
                    "User and team must belong to the same company.");
            }

            // 7. Check duplicate membership
            var alreadyExists =
                await _teamMemberRepository.ExistsAsync(
                    request.UserId,
                    request.TeamId);

            if (alreadyExists)
            {
                throw new ConflictException(
                    "User is already a member of this team.");
            }

            // 8. Create TeamMember
            var teamMember = new TeamMember
            {
                UserId = request.UserId,
                TeamId = request.TeamId,
                TeamRole =
                    (TaskFlow.Domain.Enums.TeamRole)request.TeamRole,
                IsDeleted = false
            };

            // 9. Save
            await _teamMemberRepository.AddAsync(teamMember);

            // 10. Get created record with User + Team
            var createdTeamMember =
                await _teamMemberRepository.GetByIdAsync(
                    teamMember.Id);

            if (createdTeamMember == null)
            {
                throw new Exception(
                    "Created team member could not be found.");
            }

            // 11. Return DTO
            return new TeamMemberDto
            {
                Id = createdTeamMember.Id,
                UserId = createdTeamMember.UserId,
                UserName = createdTeamMember.User?.Name ?? string.Empty,
                TeamId = createdTeamMember.TeamId,
                TeamName = createdTeamMember.Team?.Name ?? string.Empty,
                TeamRole = (int)createdTeamMember.TeamRole
            };
        }
        public async Task<bool> UpdateAsync(int id,UpdateTeamMemberRequest request)
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

            // 2. Only Admin and SuperAdmin can update TeamMembers
            if (currentUser.SystemRole != Domain.Enums.SystemRole.Admin &&
                currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to manage team members.");
            }

            // 3. Get TeamMember within current user's scope
            var existingTeamMember =
                await _teamMemberRepository.GetByIdForUserAsync(
                    id,
                    _currentUserService.UserId);

            if (existingTeamMember == null)
            {
                throw new NotFoundException(
                    $"TeamMember with ID {id} was not found.");
            }

            // 4. Update TeamRole
            existingTeamMember.TeamRole =
                (TaskFlow.Domain.Enums.TeamRole)request.TeamRole;

            await _teamMemberRepository.UpdateAsync(
                existingTeamMember);

            return true;
        }
        public async Task<bool> DeleteAsync(int id)
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

            // 2. Only Admin and SuperAdmin can delete TeamMembers
            if (currentUser.SystemRole != Domain.Enums.SystemRole.Admin &&
                currentUser.SystemRole != Domain.Enums.SystemRole.SuperAdmin)
            {
                throw new ForbiddenException(
                    "You do not have permission to manage team members.");
            }

            // 3. Get TeamMember within current user's scope
            var teamMember =
                await _teamMemberRepository.GetByIdForUserAsync(
                    id,
                    _currentUserService.UserId);

            if (teamMember == null)
            {
                throw new NotFoundException(
                    $"TeamMember with ID {id} was not found.");
            }

            // 4. Soft delete
            await _teamMemberRepository.DeleteAsync(teamMember);

            return true;
        }
    }
}