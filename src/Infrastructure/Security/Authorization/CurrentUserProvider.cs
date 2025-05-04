using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Application.Common.Security.Tokens;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using System.Text.Json;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Application.Common.Security.Roles;

namespace RuanFa.Shop.Infrastructure.Security.Authorization;

internal sealed class CurrentUserProvider(
    IUserContext userContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IDistributedCache distributedCache) : ICurrentUserProvider
{
    private const string RoleClaimsCacheKeyPrefix = "role-claims:";
    private readonly TimeSpan _roleClaimsCacheDuration = TimeSpan.FromMinutes(30);

    public async Task<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;

        if (!userContext.IsAuthenticated || userId is null)
            return null;

        var user = await userManager.FindByIdAsync(userId.Value.ToString());

        if (user == null)
        {
            return new CurrentUser(
                UserId: userId.Value,
                Email: string.Empty,
                Permissions: new List<string>().AsReadOnly(),
                Roles: new List<string>().AsReadOnly()
            );
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var permissionsSet = new HashSet<string>();

        bool isAdministrator = userRoles.Contains(Role.Administrator);

        if (isAdministrator)
        {
            permissionsSet.UnionWith(Permission.Administrator);
        }
        else
        {
            foreach (var roleName in userRoles)
            {
                var rolePermissions = await GetForRoleAsync(roleName, cancellationToken);
                permissionsSet.UnionWith(rolePermissions);
            }
        }

        return new CurrentUser(
            UserId: userId.Value,
            Email: user.Email ?? string.Empty,
            Permissions: permissionsSet.ToList().AsReadOnly(),
            Roles: userRoles.ToList().AsReadOnly()
        );
    }

    private async Task<HashSet<string>> GetForRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var cacheKey = $"{RoleClaimsCacheKeyPrefix}{roleName}";
        var cachedClaimsBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);

        List<ClaimDto>? claimDtos = null;

        if (cachedClaimsBytes != null)
        {
            claimDtos = JsonSerializer.Deserialize<List<ClaimDto>>(cachedClaimsBytes);
        }
        else
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await roleManager.GetClaimsAsync(role);
                claimDtos = claims.Select(c => new ClaimDto(c.Type, c.Value)).ToList();

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _roleClaimsCacheDuration
                };

                var bytes = JsonSerializer.SerializeToUtf8Bytes(claimDtos);
                await distributedCache.SetAsync(cacheKey, bytes, cacheOptions, cancellationToken);
            }
        }

        var permissions = new HashSet<string>();
        if (claimDtos != null)
        {
            foreach (var dto in claimDtos)
            {
                if (dto.Type == CustomClaims.Permission)
                    permissions.Add(dto.Value);
            }
        }

        return permissions;
    }

    private record ClaimDto(string Type, string Value);
}
