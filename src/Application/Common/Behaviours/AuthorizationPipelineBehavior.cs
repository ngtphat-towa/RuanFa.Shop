using ErrorOr;
using MediatR;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Application.Common.Security.Policies;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;
using System.Reflection;

namespace RuanFa.Shop.Application.Common.Behaviours;

internal sealed class AuthorizationPipelineBehavior<TRequest, TResponse>(IAuthorizationService authorizationService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IAuthorizationService _authorizationService =
        authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Skip authorization if the response type is not ErrorOr
        if (!typeof(IErrorOr).IsAssignableFrom(typeof(TResponse)))
        {
            return await next(cancellationToken);
        }

        // Get authorization attributes from the request type
        var attributes = request.GetType()
            .GetCustomAttributes<ApiAuthorizeAttribute>(inherit: true)
            .ToList();

        // Aggregate requirements from all attributes
        var requiredRoles = attributes
            .SelectMany(a => (a.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).Select(r => r.Trim()))
            .ToList();
        var requiredPermissions = attributes
            .SelectMany(a => (a.Permissions?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).Select(p => p.Trim()))
            .ToList();
        var requiredPolicies = attributes
            .SelectMany(a => (a.Policies?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).Select(p => p.Trim()))
            .ToList();

        if (attributes.Count == 0 && request is IUserMessage)
        {
            // Apply default SelfOrAdmin policy for IUserMessage without attributes
            requiredPolicies.Add(Policy.SelfOrAdmin);
        }

        if (requiredRoles.Count == 0 && requiredPermissions.Count == 0 && requiredPolicies.Count == 0)
        {
            // No attributes and not IUserMessage, proceed without authorization
            return await next(cancellationToken);
        }

        // Perform authorization
        ErrorOr<Success> authResult;

        if (request is IUserMessage userMessage)
        {
            authResult = await _authorizationService.AuthorizeUserRequest(
                userMessage,
                requiredRoles.Count != 0 ? requiredRoles : null,
                requiredPermissions.Count != 0 ? requiredPermissions : null,
                requiredPolicies.Count != 0 ? requiredPolicies : null,
                cancellationToken);
        }
        else
        {
            authResult = await _authorizationService.AuthorizeRequest(
                requiredRoles.Count != 0 ? requiredRoles : null,
                requiredPermissions.Count != 0 ? requiredPermissions : null,
                requiredPolicies.Count != 0 ? requiredPolicies : null,
                cancellationToken);
        }

        if (authResult.IsError)
        {
            return (TResponse)(dynamic)authResult.Errors;
        }

        return await next(cancellationToken);
    }
}
