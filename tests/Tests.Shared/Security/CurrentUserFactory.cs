using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.Tests.Shared.Constants;

namespace RuanFa.Shop.Tests.Shared.Security;

public static class CurrentUserFactory
{
    public static CurrentUser CreateAdminUser(
        Guid? id = null,
        string? email = null,
        IReadOnlyList<string>? permissions = null,
        IReadOnlyList<string>? roles = null)
    {
        return new CurrentUser(
            id ?? DataConstants.Accounts.AdminUserId,
            email ?? DataConstants.Accounts.AdminEmail,
            permissions ?? DataConstants.Accounts.AdminPermissions,
            roles ?? DataConstants.Accounts.AdminRoles);
    }

    public static CurrentUser CreateUser(
        Guid? id = null,
        string? email = null,
        IReadOnlyList<string>? permissions = null,
        IReadOnlyList<string>? roles = null)
    {
        return new CurrentUser(
            id ?? DataConstants.Accounts.UserId,
            email ?? DataConstants.Accounts.UserEmail,
            permissions ?? DataConstants.Accounts.UserPermissions,
            roles ?? DataConstants.Accounts.UserRoles);
    }

}
