using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using ErrorOr;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RuanFa.Shop.Application.Accounts.Enums;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Enums.Notifications;
using RuanFa.Shop.Application.Common.Extensions.Notifications;
using RuanFa.Shop.Application.Common.Notifications;
using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Application.Common.Security.Roles;
using RuanFa.Shop.Application.Common.Security.Tokens;
using RuanFa.Shop.Application.Common.Services;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Domain.Accounts.ValueObjects;
using RuanFa.Shop.Domain.Commons.Enums;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using RuanFa.Shop.Infrastructure.Accounts.Errrors;
using RuanFa.Shop.Infrastructure.Accounts.Models;
using RuanFa.Shop.Infrastructure.Security.Extensions;
using RuanFa.Shop.Infrastructure.Settings;
using Serilog;
using DomainErrors = RuanFa.Shop.Domain.Accounts.Errors.DomainErrors;

namespace RuanFa.Shop.Infrastructure.Accounts.Services;

/// <summary>
/// Provides services for managing user accounts, authentication, and credentials.
/// </summary>
internal class AccountService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<JwtSettings> jwtOption,
    IOptions<ClientSettings> clientOption,
    IOptions<SocialLoginSettings> socialLoginSettings,
    ITokenProvider tokenProvider,
    IDateTimeProvider dateTimeProvider,
    INotificationService notificationService,
    IApplicationDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    IUserContext userContext)
    : IAccountService
{
    private readonly JwtSettings _jwtSettings = jwtOption.Value;
    private readonly ClientSettings _clientSettings = clientOption.Value;
    private readonly SocialLoginSettings _socialLoginSettings = socialLoginSettings.Value;
    private const string SystemUser = "System";
    private const string UserRole = Role.User;

    /// <inheritdoc/>
    public async Task<ErrorOr<TokenResult>> AuthenticateAsync(string userIdentifier, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find user by email or username
            var appUserResult = await FindUserByIdentifierAsync(userIdentifier, cancellationToken);
            if (appUserResult.IsError)
                return appUserResult.Errors;

            var appUser = appUserResult.Value;

            // Verify email confirmation
            if (!await userManager.IsEmailConfirmedAsync(appUser))
                return InfrastructureErrors.Account.EmailNotConfirmed;

            // Attempt sign-in with password
            var result = await signInManager.CheckPasswordSignInAsync(appUser, password, lockoutOnFailure: true);
            if (result.IsLockedOut)
                return InfrastructureErrors.Account.AccountLocked;

            if (result.IsNotAllowed)
                return InfrastructureErrors.Account.SignInMethodNotAllowed;

            if (!result.Succeeded)
                return InfrastructureErrors.Account.InvalidCredential;

            // Generate access and refresh tokens
            var tokenResult = await GenerateAuthenticationResult(appUser);
            Log.Information("User {UserId} authenticated successfully.", appUser.Id);
            return tokenResult;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Authentication failed for identifier {UserIdentifier}.", userIdentifier);
            return InfrastructureErrors.Account.AuthenticationInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<Success>> ConfirmEmailAsync(string appUserId, string code, string? changedEmail = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve user by ID
            var applicationUser = await userManager.FindByIdAsync(appUserId);
            if (applicationUser is null)
                return InfrastructureErrors.Account.NotFound;


            // Retrieve associated user profile
            var userProfile = await dbContext.Profiles
                .FirstOrDefaultAsync(u => u.UserId == applicationUser.Id, cancellationToken);

            if (userProfile is null)
                return DomainErrors.UserProfile.NotFound;

            // Decode the confirmation token
            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException ex)
            {
                Log.Error(ex, "Failed to decode confirmation token for user {UserId}.", appUserId);
                return InfrastructureErrors.Account.InvalidConfirmationToken;
            }

            IdentityResult result;
            if (string.IsNullOrEmpty(changedEmail))
            {
                // Confirm the user's email
                result = await userManager.ConfirmEmailAsync(applicationUser, decodedToken);
            }
            else
            {
                // Change the user's email
                result = await userManager.ChangeEmailAsync(applicationUser, changedEmail, decodedToken);
                if (result.Succeeded)
                {
                    // Update username to match new email
                    var setUsernameResult = await userManager.SetUserNameAsync(applicationUser, changedEmail);
                    if (!setUsernameResult.Succeeded)
                    {
                        // Rollback email change on failure
                        await userManager.SetEmailAsync(applicationUser, applicationUser.Email);
                        return setUsernameResult.Errors.ToApplicationResult("ChangeEmailFailed");
                    }

                    // Update user profile with new email
                    var updateProfileResult = userProfile.UpdatePersonalDetails(
                        email: changedEmail,
                        username: userProfile.Username,
                        fullName: userProfile.FullName,
                        phoneNumber: userProfile.PhoneNumber,
                        gender: userProfile.Gender,
                        dateOfBirth: userProfile.DateOfBirth,
                        marketingConsent: userProfile.MarketingConsent);

                    if (updateProfileResult.IsError)
                        return updateProfileResult.Errors;

                    dbContext.Profiles.Update(userProfile);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            if (!result.Succeeded)
                return result.Errors.ToApplicationResult("ConfirmEmailFailed");

            Log.Information("Email confirmed or changed for user {UserId}.", appUserId);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Email confirmation failed for user {UserId}.", appUserId);
            return InfrastructureErrors.Account.ConfirmEmailInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<AccountInfoResult>> CreateAccountAsync(UserProfile profile, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate user profile
            if (string.IsNullOrWhiteSpace(profile.Email))
                return DomainErrors.UserProfile.EmailRequired;

            // Check for existing user
            var existingUser = await userManager.FindByEmailAsync(profile.Email);
            if (existingUser != null)
                return InfrastructureErrors.Account.AlreadyExists;

            // Verify default role exists
            if (!await roleManager.RoleExistsAsync(UserRole))
                return InfrastructureErrors.Account.RoleNotFound;

            var username = await GenerateUniqueUsernameAsync(profile);

            // Create identity user
            var appUser = new ApplicationUser
            {
                UserName = profile.Email,
                Email = profile.Email,
                IsActive = true,
                CreatedAt = dateTimeProvider.UtcNow,
                CreatedBy = SystemUser
            };

            // Create user account
            var createAccountResult = await userManager.CreateAsync(appUser, password);
            if (!createAccountResult.Succeeded)
                return createAccountResult.Errors.ToApplicationResult("AccountCreationFailed");

            // Assign default user role
            var assignRoleResult = await userManager.AddToRoleAsync(appUser, UserRole);
            if (!assignRoleResult.Succeeded)
            {
                // Cleanup: Delete user if role assignment fails
                await userManager.DeleteAsync(appUser);
                return assignRoleResult.Errors.ToApplicationResult("RoleAssignmentFailed");
            }

            // Assign user ID to profile and save
            profile.SetAccount(appUser.Id);
            profile.CreatedAt = dateTimeProvider.UtcNow;
            profile.CreatedBy = SystemUser;

            // Send confirmation email
            var sendEmailResult = await SendConfirmationEmailAsync(appUser, profile.Email, cancellationToken: cancellationToken);
            if (sendEmailResult.IsError)
            {
                // Cleanup: Delete user and profile if email fails
                await userManager.DeleteAsync(appUser);
                return sendEmailResult.Errors;
            }

            dbContext.Profiles.Add(profile);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Retrieve and return account details
            var accountResult = await GetAccountInfoInternalAsync(appUser.Id, cancellationToken);
            if (accountResult.IsError)
                return accountResult.Errors;

            Log.Information("Account created successfully for user {UserId}.", appUser.Id);
            return accountResult;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create account for email {Email}.", profile.Email);
            return InfrastructureErrors.Account.CreationInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<AccountInfoResult>> DeleteAccountAsync(CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        try
        {
            if (!userContext.IsAuthenticated || userId is null)
            {
                return InfrastructureErrors.Account.NotAuthenticated;
            }

            // Retrieve user
            var user = await userManager.FindByIdAsync(userId.Value.ToString());
            if (user is null)
                return InfrastructureErrors.Account.NotFound;

            // Retrieve user profile
            var userProfile = await dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (userProfile is null)
                return DomainErrors.UserProfile.NotFound;

            // Get account details before deletion
            var accountResult = await GetAccountInfoInternalAsync(userId.Value, cancellationToken);
            if (accountResult.IsError)
                return accountResult.Errors;

            // Delete user (cascades to profile via database constraints)
            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                return deleteResult.Errors.ToApplicationResult("DeleteAccountFailed");

            await dbContext.SaveChangesAsync(cancellationToken);
            Log.Information("Account deleted successfully for user {UserId}.", userId);
            return accountResult;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete account for user {UserId}.", userId);
            return InfrastructureErrors.Account.DeletionInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<Success>> ForgotPasswordAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve user
            var user = await userManager.FindByEmailAsync(email);
            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            {
                // Return success to prevent user enumeration
                return Result.Success;
            }

            // Generate and encode password reset token
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedResetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

            // Send password reset email
            var sendEmailResult = await SendPasswordResetCodeAsync(user, email, encodedResetToken, cancellationToken);
            if (sendEmailResult.IsError)
                return sendEmailResult.Errors;

            Log.Information("Password reset email sent to {Email}.", email);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initiate password reset for email {Email}.", email);
            return InfrastructureErrors.Account.ForgotPasswordInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<AccountInfoResult>> GetAccountInfoAsync(CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        try
        {
            if (!userId.HasValue || !userContext.IsAuthenticated)
            {
                return InfrastructureErrors.Account.NotAuthenticated;
            }

            return await GetAccountInfoInternalAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve account details for user {UserId}.", userId);
            return InfrastructureErrors.Account.RetrievalInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<TokenResult>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find user by refresh token
            var appUser = await userManager.Users
                .SingleOrDefaultAsync(u => u.RefreshToken == token, cancellationToken);
            if (appUser is null)
                return InfrastructureErrors.Account.RefreshTokenInvalid;

            // Verify token expiration
            if (appUser.RefreshTokenExpiryTime <= dateTimeProvider.UtcNow)
                return InfrastructureErrors.Account.RefreshTokenExpired;

            // Generate new tokens
            var tokenResult = await GenerateAuthenticationResult(appUser);
            Log.Information("Token refreshed successfully for user {UserId}.", appUser.Id);
            return tokenResult;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to refresh token.");
            return InfrastructureErrors.Account.RefreshTokenInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<Success>> ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve user
            var user = await userManager.FindByEmailAsync(email);
            if (user is null || await userManager.IsEmailConfirmedAsync(user))
            {
                // Return success to prevent user enumeration
                return Result.Success;
            }

            // Send confirmation email
            var sendEmailResult = await SendConfirmationEmailAsync(user, email, cancellationToken: cancellationToken);
            if (sendEmailResult.IsError)
                return sendEmailResult.Errors;

            Log.Information("Confirmation email resent to {Email}.", email);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to resend confirmation email to {Email}.", email);
            return InfrastructureErrors.Account.ResendConfirmationInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<Success>> ResetPasswordAsync(string email, string resetCode, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve user
            var user = await userManager.FindByEmailAsync(email);
            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
                return InfrastructureErrors.Account.ResetPasswordUserNotFound;

            // Decode reset token
            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetCode));
            }
            catch (FormatException ex)
            {
                Log.Error(ex, "Failed to decode reset token for email {Email}.", email);
                return InfrastructureErrors.Account.InvalidResetToken;
            }

            // Reset password
            var result = await userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            if (!result.Succeeded)
                return result.Errors.ToApplicationResult("ResetPasswordFailed");

            Log.Information("Password reset successfully for email {Email}.", email);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Password reset failed for email {Email}.", email);
            return InfrastructureErrors.Account.ResetPasswordInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<TokenResult>> SocialLoginAsync(SocialProvider provider, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify social token
            var payload = await VerifySocialToken(provider, token);
            if (payload is null)
                return InfrastructureErrors.Account.SocialLoginInvalidToken;

            // Check for existing user
            var user = await userManager.FindByEmailAsync(payload.Email);
            if (user is not null)
            {
                // Generate tokens for existing user
                var generatedTokenResult = await GenerateAuthenticationResult(user);
                Log.Information("Social login successful for user {UserId} via {Provider}.", user.Id, provider);
                return generatedTokenResult;
            }

            var uniqueUsername = await GenerateUniqueUsernameAsync(payload);
            // Create new user profile
            var newUserProfile = UserProfile.Create(
                userId: null,
                username: uniqueUsername,
                email: payload.Email,
                fullName: $"{payload.FirstName} {payload.LastName}".Trim(),
                phoneNumber: null,
                gender: GenderType.None,
                dateOfBirth: null,
                addresses: new List<UserAddress>(),
                preferences: new FashionPreference(),
                wishlist: new List<string>(),
                loyaltyPoints: 0,
                marketingConsent: false).Value;

            // Create account with random password
            var randomPassword = Guid.NewGuid().ToString("N");
            var accountResult = await CreateAccountAsync(newUserProfile, randomPassword, cancellationToken);
            if (accountResult.IsError)
                return accountResult.Errors;

            // Retrieve created user
            user = await userManager.FindByEmailAsync(payload.Email);
            if (user is null)
                return InfrastructureErrors.Account.NotFound;

            // Set email confirmed if verified by provider
            if (payload.IsVerified)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
            }

            // Generate tokens
            var tokenResult = await GenerateAuthenticationResult(user);
            Log.Information("Social login created new account for email {Email} via {Provider}.", payload.Email, provider);
            return tokenResult;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Social login failed for provider {Provider}.", provider);
            return InfrastructureErrors.Account.SocialLoginInternal;
        }
    }

    /// <inheritdoc/>
    public async Task<ErrorOr<Updated>> UpdateAccountCredentialAsync(
        string? newEmail, 
        string? oldPassword, 
        string? newPassword, 
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        try
        {
            if (!userContext.IsAuthenticated || userId is null)
            {
                return InfrastructureErrors.Account.NotAuthenticated;
            }

            // Retrieve user
            var user = await userManager.FindByIdAsync(userId.Value.ToString());
            if (user is null)
                return InfrastructureErrors.Account.NotFound;

            // Retrieve user profile
            var userProfile = await dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (userProfile is null)
                return DomainErrors.UserProfile.NotFound;

            var errors = new List<Error>();
            var emailUpdated = false;

            // Handle email update
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                if (string.Equals(newEmail, user.Email, StringComparison.OrdinalIgnoreCase))
                    errors.Add(InfrastructureErrors.Account.EmailSame);
                else
                {
                    var existingUser = await userManager.FindByEmailAsync(newEmail);
                    if (existingUser != null)
                        errors.Add(InfrastructureErrors.Account.AlreadyExists);
                    else
                    {
                        var sendEmailResult = await SendConfirmationEmailAsync(user, newEmail, true, cancellationToken);
                        if (sendEmailResult.IsError)
                            errors.AddRange(sendEmailResult.Errors);
                        else
                            emailUpdated = true;
                    }
                }
            }

            // Handle password update
            if (!string.IsNullOrWhiteSpace(oldPassword) && !string.IsNullOrWhiteSpace(newPassword))
            {
                if (string.Equals(oldPassword, newPassword, StringComparison.Ordinal))
                    errors.Add(InfrastructureErrors.Account.PasswordSame);
                else
                {
                    var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                    if (!result.Succeeded)
                        errors.AddRange(result.Errors.ToApplicationResult("PasswordChangeFailed"));
                }
            }

            if (errors.Count != 0)
                return errors;

            Log.Information("Credentials updated for user {UserId}. Email updated: {EmailUpdated}.", userId, emailUpdated);
            return emailUpdated
                ? InfrastructureErrors.Account.EmailConfirmationSent
                : Result.Updated;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update credentials for user {UserId}.", userId);
            return InfrastructureErrors.Account.UpdateCredentialInternal;
        }
    }

    private async Task<string> GenerateUniqueUsernameAsync(SocialPayload payload)
    {
        // 1. Generate base username from social data
        var baseUsername = !string.IsNullOrWhiteSpace(payload.FirstName)
            ? $"{payload.FirstName}{payload.LastName}".Trim().ToLower()
            : payload.Email.Split('@')[0];

        // 2. Sanitize username
        var cleanUsername = Regex.Replace(baseUsername, "[^a-z0-9]", "");

        // 3. Ensure minimum viable length
        cleanUsername = cleanUsername.Length >= 3
            ? cleanUsername
            : "user";

        // 4. Find available variant
        for (var i = 1; i <= 10; i++)
        {
            var candidate = i == 1 ? cleanUsername : $"{cleanUsername}{i - 1}";
            if (await userManager.FindByNameAsync(candidate) == null)
            {
                return candidate;
            }
        }

        // 5. Final fallback with timestamp
        return $"{cleanUsername}{DateTime.Now:yyyyMMddHHmmss}";
    }

    private async Task<string> GenerateUniqueUsernameAsync(UserProfile profile)
    {
        // 1. Generate base username from profile data
        var baseUsername = !string.IsNullOrWhiteSpace(profile.FullName)
            ? profile.FullName.Replace(" ", "").ToLower()
            : profile.Email.Split('@')[0];

        // 2. Sanitize username
        var cleanUsername = Regex.Replace(baseUsername, "[^a-z0-9]", "");

        // 3. Ensure minimum viable length
        cleanUsername = cleanUsername.Length >= 3
            ? cleanUsername
            : "user";

        // 4. Find available variant
        for (var i = 1; i <= 10; i++)
        {
            var candidate = i == 1 ? cleanUsername : $"{cleanUsername}{i - 1}";
            if (await userManager.FindByNameAsync(candidate) == null)
            {
                return candidate;
            }
        }

        // 5. Final fallback with timestamp
        return $"{cleanUsername}{DateTime.Now:yyyyMMddHHmmss}";
    }

    private async Task<ErrorOr<AccountInfoResult>> GetAccountInfoInternalAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve user
            var appUser = await userManager.FindByIdAsync(userId.ToString());
            if (appUser is null)
                return InfrastructureErrors.Account.NotFound;

            // Retrieve user profile
            var userProfile = await dbContext.Profiles
                .FirstOrDefaultAsync(m => m.UserId == appUser.Id, cancellationToken);
            if (userProfile is null)
                return DomainErrors.UserProfile.NotFound;

            // Get roles and permissions
            var permissionSet = await GetPermissionSetAsync(appUser);

            return new AccountInfoResult(
                UserId: appUser.Id,
                Email: userProfile.Email,
                FullName: userProfile.FullName,
                IsEmailVerified: appUser.EmailConfirmed,
                Created: userProfile.CreatedAt,
                LastLogin: appUser.UpdatedAt,
                Roles: permissionSet.Roles,
                Permisions: permissionSet.Permissions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve account details for user {UserId}.", userId);
            return InfrastructureErrors.Account.RetrievalInternal;
        }
    }

    /// <summary>
    /// Finds a user by their email or username.
    /// </summary>
    /// <param name="identifier">The email or username.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing the <see cref="ApplicationUser"/>, or an error if not found.</returns>
    private async Task<ErrorOr<ApplicationUser>> FindUserByIdentifierAsync(string identifier, CancellationToken cancellationToken)
    {
        // Try finding by email
        var appUser = await userManager.FindByEmailAsync(identifier);
        if (appUser is not null)
            return appUser;

        // Try finding by username
        appUser = await userManager.FindByNameAsync(identifier);
        if (appUser is not null)
            return appUser;

        return InfrastructureErrors.Account.NotFound;
    }

    /// <summary>
    /// Generates access and refresh tokens for a user.
    /// </summary>
    /// <param name="user">The user to generate tokens for.</param>
    /// <returns>A <see cref="TokenResult"/> containing the tokens and expiration.</returns>
    private async Task<TokenResult> GenerateAuthenticationResult(ApplicationUser user)
    {
        // Renew refresh token if expired or missing
        if (string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpiryTime <= dateTimeProvider.UtcNow)
        {
            user.RefreshToken = tokenProvider.CreateRefreshToken(user.Id.ToString());
            user.RefreshTokenExpiryTime = dateTimeProvider.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            await userManager.UpdateAsync(user);
        }

        // Generate access token
        var accessToken = tokenProvider.CreateAccessToken(user.Id.ToString(), user.Email);
        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken,
            ExpiresIn = (long)TimeSpan.FromMinutes(_jwtSettings.TokenExpirationInMinutes).TotalSeconds
        };
    }

    /// <summary>
    /// Sends a confirmation email to the user for email verification or change.
    /// </summary>
    /// <param name="user">The user to send the email to.</param>
    /// <param name="email">The email address to send to.</param>
    /// <param name="isChange">Indicates if this is an email change operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating success or failure.</returns>
    private async Task<ErrorOr<Success>> SendConfirmationEmailAsync(ApplicationUser user, string email, bool isChange = false, CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate confirmation token
            var code = isChange
                ? await userManager.GenerateChangeEmailTokenAsync(user, email)
                : await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encode token for URL
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var userId = await userManager.GetUserIdAsync(user);

            // Build confirmation URL
            var routeValues = new List<KeyValuePair<string, string?>>
            {
                new("userId", userId),
                new("code", encodedCode)
            };

            if (isChange)
                routeValues.Add(new("changedEmail", email));

            var confirmEmailUrl = $"{_clientSettings.ClientUri}/confirm-email?{QueryString.Create(routeValues)}";

            // Prepare and send notification
            var notificationData = NotificationDataBuilder
                .WithUseCase(NotificationUseCase.SystemActiveEmail)
                .AddParam(NotificationParameter.ActiveUrl, HtmlEncoder.Default.Encode(confirmEmailUrl))
                .AddParam(NotificationParameter.UserName, user.UserName)
                .WithReceiver(email);

           var sentNotificationResult = await notificationService.AddNotificationAsync(notificationData, cancellationToken);

            return sentNotificationResult.IsError
                ? Result.Success
                : sentNotificationResult.Errors;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send confirmation email to {Email}.", email);
            return Error.Failure("Account.SendConfirmationEmailFailed", "Failed to send confirmation email.");
        }
    }

    /// <summary>
    /// Sends a password reset email to the user.
    /// </summary>
    /// <param name="user">The user to send the email to.</param>
    /// <param name="email">The email address to send to.</param>
    /// <param name="resetCode">The encoded reset code.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating success or failure.</returns>
    private async Task<ErrorOr<Success>> SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode, CancellationToken cancellationToken = default)
    {
        try
        {
            // Build reset URL
            var routeValues = new List<KeyValuePair<string, string?>>
            {
                new("userId", await userManager.GetUserIdAsync(user)),
                new("code", resetCode)
            };

            var resetPasswordUrl = $"{_clientSettings.ClientUri}/reset-password?{QueryString.Create(routeValues)}";

            // Prepare and send notification
            var notificationData = NotificationDataBuilder
                .WithUseCase(NotificationUseCase.SystemResetPassword)
                .AddParam(NotificationParameter.ActiveUrl, HtmlEncoder.Default.Encode(resetPasswordUrl))
                .AddParam(NotificationParameter.UserName, user.UserName)
                .WithReceiver(email);

            await notificationService.AddNotificationAsync(notificationData, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send password reset email to {Email}.", email);
            return Error.Failure("Account.SendPasswordResetFailed", "Failed to send password reset email.");
        }
    }

    /// <summary>
    /// Verifies a social provider's token and extracts user information.
    /// </summary>
    /// <param name="provider">The social provider (e.g., Google, Facebook).</param>
    /// <param name="token">The social provider's token.</param>
    /// <returns>A <see cref="SocialPayload"/> with user information, or null if verification fails.</returns>
    private async Task<SocialPayload?> VerifySocialToken(SocialProvider provider, string token)
    {
        return provider switch
        {
            SocialProvider.Google => await VerifyGoogleToken(token),
            SocialProvider.Facebook => await VerifyFacebookToken(token),
            _ => null
        };
    }

    /// <summary>
    /// Verifies a Google ID token and extracts user information.
    /// </summary>
    /// <param name="idToken">The Google ID token.</param>
    /// <returns>A <see cref="SocialPayload"/> with user information, or null if verification fails.</returns>
    private async Task<SocialPayload?> VerifyGoogleToken(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_socialLoginSettings.Google.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return new SocialPayload(
                Email: payload.Email,
                IsVerified: payload.EmailVerified,
                ProviderId: payload.Subject,
                FirstName: payload.GivenName,
                LastName: payload.FamilyName,
                PictureUrl: payload.Picture);
        }
        catch (InvalidJwtException ex)
        {
            Log.Warning(ex, "Invalid Google token.");
            return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error verifying Google token.");
            return null;
        }
    }

    /// <summary>
    /// Verifies a Facebook access token and extracts user information.
    /// </summary>
    /// <param name="accessToken">The Facebook access token.</param>
    /// <returns>A <see cref="SocialPayload"/> with user information, or null if verification fails.</returns>
    private async Task<SocialPayload?> VerifyFacebookToken(string accessToken)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("FacebookAuth");
            var fbAppId = _socialLoginSettings.Facebook.AppId;
            var fbAppSecret = _socialLoginSettings.Facebook.AppSecret;

            // Validate token
            var verifyUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={fbAppId}|{fbAppSecret}";
            var verifyResponse = await httpClient.GetFromJsonAsync<FacebookTokenValidationResponse>(verifyUrl);

            if (verifyResponse?.Data == null || !verifyResponse.Data.IsValid)
            {
                Log.Warning("Facebook token validation failed: {Error}", verifyResponse?.Data?.Error?.Message ?? "Unknown error");
                return null;
            }

            // Verify app ID
            if (verifyResponse.Data.AppId != fbAppId)
            {
                Log.Warning("Facebook token is not for this app. Expected: {Expected}, Got: {Actual}", fbAppId, verifyResponse.Data.AppId);
                return null;
            }

            // Retrieve user data
            var userDataUrl = $"https://graph.facebook.com/v18.0/me?fields=id,first_name,last_name,email,picture&access_token={accessToken}";
            var userData = await httpClient.GetFromJsonAsync<FacebookUserData>(userDataUrl);

            if (userData == null || string.IsNullOrEmpty(userData.Email))
            {
                Log.Warning("Could not retrieve user email from Facebook.");
                return null;
            }

            return new SocialPayload(
                Email: userData.Email,
                IsVerified: true,
                ProviderId: userData.Id,
                FirstName: userData.FirstName,
                LastName: userData.LastName,
                PictureUrl: userData.Picture?.Data?.Url);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error verifying Facebook token.");
            return null;
        }
    }

    /// <summary>
    /// Represents a set of roles and permissions for a user.
    /// </summary>
    private record PermissionSet(List<string> Roles, List<string> Permissions);

    /// <summary>
    /// Retrieves the roles and permissions for a user.
    /// </summary>
    /// <param name="user">The user to retrieve permissions for.</param>
    /// <returns>A <see cref="PermissionSet"/> containing the user's roles and permissions.</returns>
    private async Task<PermissionSet> GetPermissionSetAsync(ApplicationUser user)
    {
        // Retrieve user roles
        var roles = (await userManager.GetRolesAsync(user)).ToList();

        // Collect permissions from roles
        var permissions = new HashSet<string>();
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                var claims = await roleManager.GetClaimsAsync(role);
                var rolePermissions = claims
                    .Where(c => c.Type == CustomClaims.Permission)
                    .Select(c => c.Value);
                permissions.UnionWith(rolePermissions);
            }
        }

        return new PermissionSet(roles, permissions.ToList());
    }
}
