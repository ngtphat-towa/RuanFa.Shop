using ErrorOr;
using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Application.Common.Security.Policies;
using RuanFa.Shop.Application.Common.Security.Roles;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Infrastructure.Security.Authorization;

public class PolicyEnforcer : IPolicyEnforcer
{
    public ErrorOr<Success> Authorize(IUserMessage? request, CurrentUser? user, string policy)
    {
        if (user == null)
        {
            return Error.Unauthorized(code: "Authorization.NoUser", description: "No authenticated user.");
        }

        return policy switch
        {
            Policy.SelfOrAdmin => SelfOrAdminPolicy(request, user),
            _ => Error.Unexpected(
                code: "Authorization.UnknownPolicy",
                description: $"Unknown policy name: '{policy}'."),
        };
    }

    private static ErrorOr<Success> SelfOrAdminPolicy(IUserMessage? request, CurrentUser user)
    {
        if (request == null)
        {
            return Error.Unauthorized(
                code: "Authorization.InvalidRequest",
                description: "SelfOrAdmin policy requires a user-specific request.");
        }

        bool isSelf = request.UserId == user.UserId;
        bool isAdmin = user.Roles.Contains(Role.Administrator) || user.Permissions.Contains("AdminAccess");

        return isSelf || isAdmin
            ? Result.Success
            : Error.Unauthorized(
                code: "Authorization.SelfOrAdminFailed",
                description: "User must be the request owner or have admin privileges.");
    }
}
