using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Common.Security.Authorization.Services;

/// <summary>
/// Defines a service for authorizing requests based on user roles, permissions, and policies.
/// </summary>
public interface IAuthorizationService
{
    Task<ErrorOr<Success>> AuthorizeRequest(
        List<string>? requiredRoles,
        List<string>? requiredPermissions,
        List<string>? requiredPolicies,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> AuthorizeUserRequest(
        IUserMessage request,
        List<string>? requiredRoles,
        List<string>? requiredPermissions,
        List<string>? requiredPolicies,
        CancellationToken cancellationToken = default);
}
