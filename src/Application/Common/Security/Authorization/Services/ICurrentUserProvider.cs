using RuanFa.Shop.Application.Common.Security.Authorization.Models;

namespace RuanFa.Shop.Application.Common.Security.Authorization.Services;

/// <summary>
/// Defines a service for retrieving the current user’s information.
/// </summary>
public interface ICurrentUserProvider
{
    Task<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken);
}
