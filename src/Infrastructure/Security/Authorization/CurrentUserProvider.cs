using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Application.Common.Security.Tokens;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using System.Text.Json;
using System.Threading;

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
        if (userContext.UserId == null || !userContext.IsAuthenticated)
            return null;

        var userId = userContext.UserId;
        var user = await userManager.FindByIdAsync(userId.ToString() ?? "");

        if (user == null)
        {
            return new CurrentUser(
                UserId: userId,
                Email: string.Empty,
                Permissions: new List<string>().AsReadOnly(),
                Roles: new List<string>().AsReadOnly()
            );
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var permissionsSet = new HashSet<string>();

        foreach (var roleName in userRoles)
        {
            var rolePermissions = await GetForRoleAsync(roleName, cancellationToken);
            permissionsSet.UnionWith(rolePermissions);
        }

        return new CurrentUser(
            UserId: userId,
            Email: user.Email ?? string.Empty,
            Permissions: permissionsSet.ToList().AsReadOnly(),
            Roles: userRoles.ToList().AsReadOnly()
        );
    }

    private async Task<HashSet<string>> GetForRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var cacheKey = $"{RoleClaimsCacheKeyPrefix}{roleName}";
        var cachedClaimsBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);

        List<Claim>? roleClaims = null;
        if (cachedClaimsBytes != null)
        {
            roleClaims = JsonSerializer.Deserialize<List<Claim>>(cachedClaimsBytes);
        }
        else
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role != null)
            {
                var roleClaimsList = await roleManager.GetClaimsAsync(role);
                roleClaims = [.. roleClaimsList];

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _roleClaimsCacheDuration
                };

                var claimsBytesToCache = JsonSerializer.SerializeToUtf8Bytes(roleClaims);
                await distributedCache.SetAsync(cacheKey, claimsBytesToCache, cacheEntryOptions, cancellationToken);
            }
        }

        HashSet<string> permissionsSet = [];

        if (roleClaims != null)
        {
            foreach (var claim in roleClaims)
            {
                if (claim.Type == CustomClaims.Permission)
                {
                    permissionsSet.Add(claim.Value);
                }
            }
        }

        return permissionsSet;
    }
}
