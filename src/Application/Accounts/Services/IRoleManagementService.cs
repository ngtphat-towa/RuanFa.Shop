using ErrorOr;
using RuanFa.Shop.Application.Accounts.Models;

namespace RuanFa.Shop.Application.Accounts.Services;

public interface IRoleManagementService
{
    // Role Management
    Task<ErrorOr<string>> CreateRoleAsync(string roleName, List<string>? permissions = null, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteRoleAsync(string roleNameOrId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateRoleAsync(string roleNameOrId, string newName, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AssignRolePermissionAsync(string roleNameOrId, List<string>? permissions, CancellationToken cancellationToken = default);
    Task<ErrorOr<List<RoleResult>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<RoleDetailResult>> GetRoleByIdOrName(string roleNameOrId, CancellationToken cancellationToken = default);

    // User Role Management
    Task<ErrorOr<Success>> AssignRolesToUserAsync(string userId, List<string> roleNameOrIds, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AssignRoleToUserAsync(string userId, string roleNameOrId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> RemoveRoleFromUserAsync(string userId, string roleNameOrId, CancellationToken cancellationToken = default);
    Task<ErrorOr<List<RoleResult>>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
}
