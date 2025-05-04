using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.Application.Common.Security.Roles;
using RuanFa.Shop.Application.Common.Security.Tokens;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using RuanFa.Shop.Tests.Shared.Constants;

namespace RuanFa.Shop.Application.SubcutaneousTests.Commons;

public static class WebAppFactoryExtensions
{
    /// <summary>
    /// Creates a test user and sets it up in the test user context and provider
    /// </summary>
    public static async Task<(ApplicationUser User, UserProfile Profile)> CreateTestUserAsync(
      this WebAppFactory factory,
      string? role = null,
      ApplicationUser? user = null,
      UserProfile? profile = null,
      string? password = null)
    {
        var userManager = factory.UserManager;
        var roleManager = factory.RoleManager;
        var dbContext = factory.DbContext;

        var applicationUser = user ?? DataConstants.Accounts.DefaultAdminUser;
        var applicationPassword = password ?? DataConstants.Accounts.Password;

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            // Step 1: Create ApplicationUser
            var createResult = await userManager.CreateAsync(applicationUser, applicationPassword);
            if (!createResult.Succeeded)
            {
                throw new Exception($"Failed to create test user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            // Step 2: Create or Update UserProfile with the correct UserId
            var profileUser = profile ?? DataConstants.UserProfiles.DefaultAdminProfile;
            // Set the UserId to the newly created user's ID
            profileUser.SetAccount(applicationUser.Id);
            await dbContext.Profiles.AddAsync(profileUser);
            await dbContext.SaveChangesAsync();

            // Step 3: Add role
            var roleName = role ?? Role.Administrator;
            await roleManager.CreateAsync(new ApplicationRole(roleName));
            await userManager.AddToRoleAsync(applicationUser, roleName);

            // Step 4: Set up the test user context
            var permissions = await GetPermissionsForRole(factory, roleName);
            var currentUser = new CurrentUser
            (
                applicationUser.Id,
                applicationUser.Email!,
                permissions,
                new List<string> { roleName }
            );

            factory.TestUserContext.SetUserContext(applicationUser.Id, applicationUser.Email);
            factory.TestCurrentUserProvider.SetReturn(currentUser);

            await transaction.CommitAsync();
            return (applicationUser, profileUser);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Gets the permissions associated with a role
    /// </summary>
    private static async Task<List<string>> GetPermissionsForRole(WebAppFactory factory, string role)
    {
        var serviceScope = factory.Services.CreateScope();
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var applicationRole = await roleManager.FindByNameAsync(role);
        if (applicationRole == null)
            return new List<string>();

        var claims = await roleManager.GetClaimsAsync(applicationRole);
        return [.. claims.Where(c => c.Type == CustomClaims.Permission).Select(c => c.Value)];
    }
}
