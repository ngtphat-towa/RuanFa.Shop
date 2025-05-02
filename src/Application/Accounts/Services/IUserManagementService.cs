using ErrorOr;
using RuanFa.Shop.Domain.Accounts.Entities;

namespace RuanFa.Shop.Application.Accounts.Services;

public interface IUserManagementService
{
    Task<ErrorOr<UserProfile>> GetUserProfileByIdAsync(
        string userId, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<UserProfile>> GetUserProfileByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<UserProfile>> CreateUserAsync(
        UserProfile userProfile, 
        string password, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> UpdateUserProfileAsync(
        string userId, 
        UserProfile userProfile, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> DeleteUserAsync(
        string userId, 
        CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeactivateUserAsync(
        string userId, 
        CancellationToken cancellationToken = default);
}
