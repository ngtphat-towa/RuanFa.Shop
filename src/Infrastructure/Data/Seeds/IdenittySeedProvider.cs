using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Application.Common.Security.Roles;
using RuanFa.Shop.Application.Common.Security.Tokens;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Domain.Accounts.ValueObjects;
using RuanFa.Shop.Domain.Commons.Enums;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using Serilog;
using System.Security.Claims;

namespace RuanFa.Shop.Infrastructure.Data.Seeds
{
    public class IdentitySeedProvider : IDataSeeder
    {
        private readonly IServiceProvider _serviceProvider;

        public IdentitySeedProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Log.Information("[IdentitySeed] Starting role and user seeding...");

            try
            {
                await SeedRolesAsync(roleManager, cancellationToken);
                await SeedUsersAsync(userManager, dbContext, cancellationToken);

                Log.Information("[IdentitySeed] All seeding operations completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[IdentitySeed] Error during seeding process.");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, CancellationToken cancellationToken)
        {
            Log.Information("[IdentitySeed:Roles] Seeding roles started.");

            var roles = new Dictionary<string, List<string>>
            {
                { Role.Administrator, Permission.Administrator },
                { Role.User, Permission.User }
            };

            foreach (var kvp in roles)
            {
                string roleName = kvp.Key;
                var permissions = kvp.Value;

                Log.Debug("[IdentitySeed:Roles] Processing role {RoleName} with {PermissionCount} permissions.", roleName, permissions.Count);

                try
                {
                    var appRole = await roleManager.FindByNameAsync(roleName);
                    if (appRole == null)
                    {
                        Log.Warning("[IdentitySeed:Roles] Role {RoleName} not found. Creating new role.", roleName);
                        appRole = new ApplicationRole(roleName);
                        var result = await roleManager.CreateAsync(appRole);
                        if (!result.Succeeded)
                        {
                            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                            Log.Error("[IdentitySeed:Roles] Failed to create role {RoleName}. Errors: {Errors}", roleName, errors);
                            continue;
                        }
                        Log.Information("[IdentitySeed:Roles] Role {RoleName} created successfully.", roleName);
                    }
                    else
                    {
                        Log.Information("[IdentitySeed:Roles] Role {RoleName} already exists.", roleName);
                    }

                    await AssignRolePermissionsAsync(roleManager, appRole, permissions, cancellationToken);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[IdentitySeed:Roles] Exception occurred while processing role {RoleName}", roleName);
                    // Continue with other roles instead of failing the entire seeding process
                }
            }

            Log.Information("[IdentitySeed:Roles] Role seeding finished.");
        }

        private static async Task AssignRolePermissionsAsync(
            RoleManager<ApplicationRole> roleManager,
            ApplicationRole role,
            List<string> permissions,
            CancellationToken cancellationToken)
        {
            if (role == null)
            {
                Log.Error("[IdentitySeed:Roles] Cannot assign permissions to null role.");
                return;
            }

            Log.Debug("[IdentitySeed:Roles] Assigning permissions to role {RoleName}.", role.Name);
            var existingClaims = await roleManager.GetClaimsAsync(role);
            var existingPermissions = existingClaims
                .Where(c => c.Type == CustomClaims.Permission)
                .Select(c => c.Value)
                .ToHashSet();

            var toAdd = permissions.Except(existingPermissions).ToList();
            var toRemove = existingPermissions.Except(permissions).ToList();

            Log.Debug("[IdentitySeed:Roles] Permissions to add: {AddCount}, to remove: {RemoveCount} for role {RoleName}.",
                toAdd.Count, toRemove.Count, role.Name);

            foreach (var permission in toAdd)
            {
                try
                {
                    var result = await roleManager.AddClaimAsync(role, new Claim(CustomClaims.Permission, permission));
                    if (!result.Succeeded)
                    {
                        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                        Log.Error("[IdentitySeed:Roles] Failed to add permission {Permission} to role {RoleName}. Errors: {Errors}",
                            permission, role.Name, errors);
                    }
                    else
                    {
                        Log.Information("[IdentitySeed:Roles] Added permission {Permission} to role {RoleName}.", permission, role.Name);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[IdentitySeed:Roles] Exception while adding permission {Permission} to role {RoleName}",
                        permission, role.Name);
                }
            }

            foreach (var permission in toRemove)
            {
                try
                {
                    var claim = existingClaims.FirstOrDefault(c => c.Value == permission);
                    if (claim != null)
                    {
                        var result = await roleManager.RemoveClaimAsync(role, claim);
                        if (result.Succeeded)
                        {
                            Log.Information("[IdentitySeed:Roles] Removed outdated permission {Permission} from role {RoleName}.",
                                permission, role.Name);
                        }
                        else
                        {
                            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                            Log.Error("[IdentitySeed:Roles] Failed to remove permission {Permission} from role {RoleName}. Errors: {Errors}",
                                permission, role.Name, errors);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[IdentitySeed:Roles] Exception while removing permission {Permission} from role {RoleName}",
                        permission, role.Name);
                }
            }
        }

        private async Task SeedUsersAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            Log.Information("[IdentitySeed:Users] Seeding users started.");

            var users = new List<(string Email, string Password, string FullName, string Phone, GenderType Gender, DateTime BirthDateUtc, string Role)>
            {
                ("admin1@quingfa.com", "Admin1Password!", "Admin One", "1234567890", GenderType.Male, new DateTime(1990, 1, 1), Role.Administrator),
                ("admin2@quingfa.com", "Admin2Password!", "Admin Two", "1234567891", GenderType.Male, new DateTime(1990, 2, 2), Role.Administrator),
                ("user1@quingfa.com", "User1Password!", "User One", "0987654321", GenderType.Female, new DateTime(1995, 5, 5), Role.User),
                ("user2@quingfa.com", "User2Password!", "User Two", "0987654322", GenderType.Female, new DateTime(1995, 6, 6), Role.User)
            };

            foreach (var userInfo in users)
            {
                Log.Debug("[IdentitySeed:Users] Processing user {Email}.", userInfo.Email);
                try
                {
                    await CreateOrUpdateUserAsync(
                        userManager,
                        dbContext,
                        userInfo.Email,
                        userInfo.Password,
                        userInfo.FullName,
                        userInfo.Phone,
                        userInfo.Gender,
                        userInfo.BirthDateUtc,
                        userInfo.Role,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[IdentitySeed:Users] Exception while processing user {Email}", userInfo.Email);
                    // Continue with other users instead of failing the entire seeding process
                }
            }

            Log.Information("[IdentitySeed:Users] User seeding finished.");
        }

        private static async Task CreateOrUpdateUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            string email,
            string password,
            string fullName,
            string phone = "",
            GenderType genderType = GenderType.None,
            DateTime birthDateUtc = default,
            string role = Role.User,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                Log.Error("[IdentitySeed:Users] Email cannot be null or empty.");
                return;
            }

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                await UpdateExistingUserAsync(userManager, dbContext, existingUser, fullName, phone, role, cancellationToken);
                return;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = phone,
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "IdentitySeed",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "IdentitySeed"
            };

            Log.Information("[IdentitySeed:Users] Creating user '{Email}'.", email);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    Log.Error("[IdentitySeed:Users] Failed to create user {Email}. Errors: {Errors}", email, errors);
                    await transaction.RollbackAsync(cancellationToken);
                    return;
                }
                Log.Information("[IdentitySeed:Users] User '{Email}' created in Identity store.", email);

                var profileResult = await CreateUserProfileAsync(
                    dbContext,
                    user.Id,
                    email,
                    fullName,
                    phone,
                    genderType,
                    birthDateUtc,
                    cancellationToken);

                if (profileResult.profile == null)
                {
                    Log.Error("[IdentitySeed:Users] Failed to create profile for user {Email}.", email);
                    await transaction.RollbackAsync(cancellationToken);
                    return;
                }

                Log.Debug("[IdentitySeed:Users] UserProfile created with ID {ProfileId} for user {Email}.",
                    profileResult.profile.Id, email);

                var roleResult = await userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    Log.Error("[IdentitySeed:Users] Failed to assign role {Role} to user {Email}. Errors: {Errors}",
                        role, email, errors);
                    await transaction.RollbackAsync(cancellationToken);
                    return;
                }

                Log.Information("[IdentitySeed:Users] Assigned role '{Role}' to user '{Email}'.", role, email);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Log.Information("[IdentitySeed:Users] Transaction committed for user '{Email}'.", email);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Log.Error(ex, "[IdentitySeed:Users] Exception while seeding user '{Email}'. Rolling back.", email);
                throw;
            }
        }

        private static async Task UpdateExistingUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            ApplicationUser user,
            string fullName,
            string phone,
            string role,
            CancellationToken cancellationToken)
        {
            Log.Warning("[IdentitySeed:Users] User '{Email}' already exists. Checking profile and roles.", user.Email);

            // Update phone if different
            if (user.PhoneNumber != phone)
            {
                user.PhoneNumber = phone;
                await userManager.UpdateAsync(user);
            }

            // Check if user has the required role
            var userRoles = await userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);
                if (roleResult.Succeeded)
                {
                    Log.Information("[IdentitySeed:Users] Added missing role '{Role}' to existing user '{Email}'.",
                        role, user.Email);
                }
                else
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    Log.Error("[IdentitySeed:Users] Failed to add role {Role} to existing user {Email}. Errors: {Errors}",
                        role, user.Email, errors);
                }
            }

            // Check if user has a profile
            var existingProfile = await dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);

            if (existingProfile == null)
            {
                Log.Warning("[IdentitySeed:Users] Profile not found for existing user '{Email}'. Creating profile.", user.Email);
                await CreateUserProfileAsync(
                    dbContext,
                    user.Id,
                    user.Email!,
                    fullName,
                    phone,
                    GenderType.None,
                    DateTime.UtcNow,
                    cancellationToken);
            }
            else if (existingProfile.FullName != fullName)
            {
                Log.Information("[IdentitySeed:Users] Updating fullname for existing user '{Email}'.", user.Email);
                var updateResult = existingProfile.UpdatePersonalDetails(
                    email: existingProfile.Email,
                    username: existingProfile.Username,
                    fullName: fullName,
                    phoneNumber: existingProfile.PhoneNumber,
                    gender: existingProfile.Gender,
                    dateOfBirth: existingProfile.DateOfBirth,
                    marketingConsent: existingProfile.MarketingConsent);

                if (!updateResult.IsError)
                {
                    dbContext.Profiles.Update(existingProfile);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    Log.Information("[IdentitySeed:Users] Profile updated for user '{Email}'.", user.Email);
                }
                else
                {
                    Log.Error("[IdentitySeed:Users] Failed to update profile for user '{Email}'. Error: {Error}",
                        user.Email, updateResult.FirstError);
                }
            }
        }

        private static async Task<(UserProfile? profile, bool success)> CreateUserProfileAsync(
            ApplicationDbContext dbContext,
            Guid applicationUserId,
            string email,
            string fullName,
            string phone,
            GenderType genderType,
            DateTime birthDateUtc,
            CancellationToken cancellationToken)
        {
            Log.Debug("[IdentitySeed:Users] Creating profile for {Email}.", email);

            // Create addresses with error handling
            var addresses = new List<UserAddress>();
            foreach (var addressInfo in GetDefaultAddresses())
            {
                var addressResult = UserAddress.Create(
                    addressLine1: addressInfo.addressLine1,
                    addressLine2: addressInfo.addressLine2,
                    city: addressInfo.city,
                    state: addressInfo.state,
                    country: addressInfo.country,
                    postalCode: addressInfo.postalCode,
                    boutiquePickupLocation: addressInfo.boutiquePickupLocation,
                    deliveryInstructions: addressInfo.deliveryInstructions,
                    type: addressInfo.type,
                    isDefault: addressInfo.isDefault);

                if (addressResult != null)
                {
                    addresses.Add(addressResult);
                }
                else
                {
                    Log.Warning("[IdentitySeed:Users] Failed to create address for {Email}", email);
                }
            }

            // Create user profile with error handling
            var profileResult = UserProfile.Create(
                userId: applicationUserId,
                username: email.Split('@')[0],  // Generate username from email
                email: email,
                fullName: fullName,
                phoneNumber: phone,
                gender: genderType,
                dateOfBirth: birthDateUtc != default ? new DateTimeOffset(birthDateUtc, TimeSpan.Zero) : null,
                addresses: addresses,
                preferences: GetDefaultPreferences(),
                wishlist: GetDefaultWishlist(),
                loyaltyPoints: 0,
                marketingConsent: true
            );

            if (profileResult.IsError)
            {
                Log.Error("[IdentitySeed:Users] Failed to create profile for {Email}. Error: {Error}",
                    email, profileResult.FirstError);
                return (null, false);
            }

            var profile = profileResult.Value;
            profile.CreatedAt = DateTime.UtcNow;
            profile.CreatedBy = "IdentitySeed";
            profile.UpdatedAt = DateTime.UtcNow;
            profile.UpdatedBy = "IdentitySeed";

            dbContext.Profiles.Add(profile);
            await dbContext.SaveChangesAsync(cancellationToken);
            Log.Debug("[IdentitySeed:Users] Profile persisted for {Email} with ID {ProfileId}.", email, profile.Id);
            return (profile, true);
        }

        private static List<(string addressLine1, string addressLine2, string city, string state, string country,
            string postalCode, string boutiquePickupLocation, string deliveryInstructions, AddressType type, bool isDefault)>
            GetDefaultAddresses()
        {
            return new List<(string, string, string, string, string, string, string, string, AddressType, bool)>
            {
                ("1 Apple Park Way", "", "Cupertino", "CA", "USA", "95014",
                    "Apple Park Visitor Center", "Leave at the front desk", AddressType.Shipping, true),

                ("1 Apple Park Way", "", "Cupertino", "CA", "USA", "95014",
                    "Apple Park Visitor Center", "Leave at the front desk", AddressType.Billing, false)
            };
        }

        private static FashionPreference GetDefaultPreferences()
        {
            var preferenceResult = FashionPreference.Create(
                clothingSize: "M",
                favoriteCategories: new List<string> { "Shirts", "Pants", "Accessories" }
            );

            return preferenceResult ?? new FashionPreference();
        }

        private static List<string> GetDefaultWishlist()
        {
            return new List<string>
            {
                "Classic T-Shirt",
                "Blue Jeans",
                "Leather Jacket"
            };
        }
    }
}
