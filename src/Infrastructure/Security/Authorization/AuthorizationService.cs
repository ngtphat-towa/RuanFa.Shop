using ErrorOr;
using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Application.Common.Security.Policies;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Infrastructure.Security.Authorization;

public class AuthorizationService(
    IPolicyEnforcer policyEnforcer,
    ICurrentUserProvider currentUserProvider) 
    : IAuthorizationService
{
    private readonly IPolicyEnforcer _policyEnforcer = 
        policyEnforcer ?? throw new ArgumentNullException(nameof(policyEnforcer));
    private readonly ICurrentUserProvider _currentUserProvider = 
        currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));

    public async Task<ErrorOr<Success>> AuthorizeRequest(
        List<string>? requiredRoles,
        List<string>? requiredPermissions,
        List<string>? requiredPolicies,
        CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserProvider.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
        {
            return Error.Unauthorized(
                code: "Authorization.NoUser", 
                description: "No authenticated user found.");
        }

        return await AuthorizeCore(null, currentUser, requiredRoles, requiredPermissions, requiredPolicies);
    }

    public async Task<ErrorOr<Success>> AuthorizeUserRequest(
        IUserMessage request,
        List<string>? requiredRoles,
        List<string>? requiredPermissions,
        List<string>? requiredPolicies,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await _currentUserProvider.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
        {
            return Error.Unauthorized(
                code: "Authorization.NoUser", 
                description: "No authenticated user found.");
        }

        // Add SelfOrAdmin policy for user-specific requests
        var policies = requiredPolicies?.ToList() ?? new List<string>();
        if (!policies.Contains(Policy.SelfOrAdmin))
        {
            policies.Add(Policy.SelfOrAdmin);
        }

        return await AuthorizeCore(request, currentUser, requiredRoles, requiredPermissions, policies);
    }

    private async Task<ErrorOr<Success>> AuthorizeCore(
        IUserMessage? request,
        CurrentUser currentUser,
        List<string>? requiredRoles,
        List<string>? requiredPermissions,
        List<string>? requiredPolicies)
    {
        await Task.CompletedTask;
        // Check required permissions
        if (requiredPermissions?.Any() == true &&
            requiredPermissions.Except(currentUser.Permissions).Any())
        {
            return Error.Unauthorized(
                code: "Authorization.NoPermissions", 
                description: "User lacks required permissions for this action.");
        }

        // Check required roles
        if (requiredRoles?.Any() == true && 
            requiredRoles.Except(currentUser.Roles).Any())
        {
            return Error.Unauthorized(
                code: "Authorization.NoRoles", 
                description: "User lacks required roles for this action.");
        }

        // Evaluate required policies
        if (requiredPolicies?.Any() == true)
        {
            foreach (var policy in requiredPolicies)
            {
                var policyResult = _policyEnforcer.Authorize(request, currentUser, policy);
                if (policyResult.IsError)
                {
                    return policyResult.Errors;
                }
            }
        }

        return Result.Success;
    }
}
