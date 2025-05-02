using ErrorOr;
using RuanFa.Shop.Application.Accounts.Enums;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Domain.Accounts.Entities;

namespace RuanFa.Shop.Application.Accounts.Services;

/// <summary>
/// Defines the contract for managing user accounts, authentication, and credentials.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Authenticates a user using their email or username and password.
    /// </summary>
    /// <param name="userIdentifier">The user's email address or username.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing a <see cref="TokenResult"/> with access and refresh tokens, or an error if authentication fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<TokenResult>> AuthenticateAsync(
        string userIdentifier,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user using a social provider's token (e.g., Google, Facebook).
    /// </summary>
    /// <param name="provider">The social provider (e.g., Google, Facebook).</param>
    /// <param name="token">The social provider's access or ID token.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing a <see cref="TokenResult"/> with access and refresh tokens, or an error if authentication fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<TokenResult>> SocialLoginAsync(
        SocialProvider provider,
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    /// <param name="token">The refresh token.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing a new <see cref="TokenResult"/>, or an error if the refresh token is invalid or expired.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<TokenResult>> RefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a password reset by sending a reset code to the user's email.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating success, or an error if the operation fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<Success>> ForgotPasswordAsync(
        string email,
        CancellationToken cancellationToken);

    /// <summary>
    /// Resets a user's password using a reset code.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="resetCode">The password reset code sent to the user.</param>
    /// <param name="newPassword">The new password.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating success, or an error if the reset fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<Success>> ResetPasswordAsync(
        string email,
        string resetCode,
        string newPassword,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a user's email address or changes it to a new email.
    /// </summary>
    /// <param name="appUserId">The user's unique identifier.</param>
    /// <param name="code">The email confirmation or change token.</param>
    /// <param name="changedEmail">The new email address, if changing the email.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating success, or an error if confirmation fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<Success>> ConfirmEmailAsync(
        string appUserId,
        string code,
        string? changedEmail = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resends a confirmation email to the user if their email is unconfirmed.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating success, or an error if the operation fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<Success>> ResendConfirmationEmailAsync(
        string email,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new user account with the specified profile and password.
    /// </summary>
    /// <param name="profile">The user's profile information.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing an <see cref="AccountInfoResult"/> with account details, or an error if creation fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<AccountInfoResult>> CreateAccountAsync(
        UserProfile profile,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information about a user's account.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing an <see cref="AccountInfoResult"/>, or an error if the user is not found.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<AccountInfoResult>> GetAccountInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user's account and associated profile.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing an <see cref="AccountInfoResult"/> of the deleted account, or an error if deletion fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<AccountInfoResult>> DeleteAccountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user's email or password.
    /// </summary>
    /// <param name="newEmail">The new email address, if updating.</param>
    /// <param name="oldPassword">The current password, if updating the password.</param>
    /// <param name="newPassword">The new password, if updating.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating the update status, or an error if the update fails.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ErrorOr<Updated>> UpdateAccountCredentialAsync(
        string? newEmail,
        string? oldPassword,
        string? newPassword,
        CancellationToken cancellationToken = default);
}
