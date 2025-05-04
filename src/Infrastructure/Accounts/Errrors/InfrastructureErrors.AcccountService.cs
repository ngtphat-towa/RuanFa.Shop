using ErrorOr;

namespace RuanFa.Shop.Infrastructure.Accounts.Errrors;

/// <summary>
/// Defines error conditions for account-related operations in the infrastructure layer.
/// </summary>
public static partial class InfrastructureErrors
{
    /// <summary>
    /// Contains errors specific to account management operations.
    /// </summary>
    public static class Account
    {
        /// <summary>
        /// Gets an error indicating that a user was not found.
        /// </summary>
        public static Error NotFound => Error.NotFound(
            code: "Account.NotFound",
            description: "The specified user could not be found.");

        /// <summary>
        /// Gets an error indicating that the user's email is not confirmed.
        /// </summary>
        public static Error EmailNotConfirmed => Error.Validation(
            code: "Account.EmailNotConfirmed",
            description: "The email address has not been verified. Please confirm your email.");

        /// <summary>
        /// Gets an error indicating that the account is temporarily locked.
        /// </summary>
        public static Error AccountLocked => Error.Unauthorized(
            code: "Account.AccountLocked",
            description: "The account is temporarily locked. Please contact support.");

        /// <summary>
        /// Gets an error indicating that the sign-in method is not allowed.
        /// </summary>
        public static Error SignInMethodNotAllowed => Error.Unauthorized(
            code: "Account.SignInMethodNotAllowed",
            description: "The sign-in method is not permitted for this account.");

        /// <summary>
        /// Gets an error indicating that the provided credentials are invalid.
        /// </summary>
        public static Error InvalidCredential => Error.Unauthorized(
            code: "Account.InvalidCredential",
            description: "The email or password is incorrect. Please try again.");

        /// <summary>
        /// Gets an error indicating an internal failure during authentication.
        /// </summary>
        public static Error AuthenticationInternal => Error.Failure(
            code: "Account.AuthenticationInternal",
            description: "An unexpected error occurred during authentication. Please try again later.");

        /// <summary>
        /// Gets an error indicating that a user with the specified email already exists.
        /// </summary>
        public static Error AlreadyExists => Error.Conflict(
            code: "Account.AlreadyExists",
            description: "A user with this email address already exists.");

        public static Error UsernameTaken => Error.Conflict(
             code: "Account.UsernameTaken",
             description: "The requested username is already in use");

        /// <summary>
        /// Gets an error indicating an internal failure during account creation.
        /// </summary>
        public static Error CreationInternal => Error.Failure(
            code: "Account.CreationInternal",
            description: "An unexpected error occurred while creating the account.");

        /// <summary>
        /// Gets an error indicating that the new email is the same as the current email.
        /// </summary>
        public static Error EmailSame => Error.Validation(
            code: "Account.EmailSame",
            description: "The new email address must be different from the current email.");

        /// <summary>
        /// Gets an error indicating that the new password is the same as the current password.
        /// </summary>
        public static Error PasswordSame => Error.Validation(
            code: "Account.PasswordSame",
            description: "The new password must be different from the current password.");

        /// <summary>
        /// Gets an error indicating that a confirmation email has been sent for email verification.
        /// </summary>
        public static Error EmailConfirmationSent => Error.Custom(
            type: 302,
            code: "Account.EmailConfirmationSent",
            description: "A confirmation email has been sent. Please verify your new email address.");

        /// <summary>
        /// Gets an error indicating an internal failure during email confirmation.
        /// </summary>
        public static Error ConfirmEmailInternal => Error.Failure(
            code: "Account.ConfirmEmailInternal",
            description: "An unexpected error occurred while confirming the email.");

        public static Error UsernameGenerationFailed => Error.Conflict(
            code: "Account.UsernameGenerationFailed",
            description: "Failed to generate a unique username after multiple attempts");

        /// <summary>
        /// Gets an error indicating that the default user role was not found.
        /// </summary>
        public static Error RoleNotFound => Error.NotFound(
            code: "Account.RoleNotFound",
            description: "The default user role is not configured in the system.");

        /// <summary>
        /// Gets an error indicating an internal failure during password reset initiation.
        /// </summary>
        public static Error ForgotPasswordInternal => Error.Failure(
            code: "Account.ForgotPasswordInternal",
            description: "An unexpected error occurred while initiating the password reset.");

        /// <summary>
        /// Gets an error indicating that the user was not found during password reset.
        /// </summary>
        public static Error ResetPasswordUserNotFound => Error.NotFound(
            code: "Account.ResetPasswordUserNotFound",
            description: "The specified user could not be found or the email is not verified.");


        /// <summary>
        /// Gets an error indicating that the password reset token is invalid or the user was not found.
        /// </summary>
        public static Error InvalidResetToken => Error.Unauthorized(
            code: "Account.InvalidResetToken",
            description: "The password reset token is invalid.");

        /// <summary>
        /// Gets an error indicating that the confirmation token is invalid.
        /// </summary>
        public static Error InvalidConfirmationToken => Error.Unauthorized(
            code: "Account.InvalidConfirmationToken",
            description: "The confirmation token is invalid.");

        /// <summary>
        /// Gets an error indicating an internal failure during password reset.
        /// </summary>
        public static Error ResetPasswordInternal => Error.Failure(
            code: "Account.ResetPasswordInternal",
            description: "An unexpected error occurred while resetting the password.");

        /// <summary>
        /// Gets an error indicating that the refresh token is invalid.
        /// </summary>
        public static Error RefreshTokenInvalid => Error.Unauthorized(
            code: "Account.RefreshTokenInvalid",
            description: "The provided refresh token is invalid.");

        /// <summary>
        /// Gets an error indicating that the refresh token has expired.
        /// </summary>
        public static Error RefreshTokenExpired => Error.Unauthorized(
            code: "Account.RefreshTokenExpired",
            description: "The refresh token has expired. Please sign in again.");

        /// <summary>
        /// Gets an error indicating an internal failure during token refresh.
        /// </summary>
        public static Error RefreshTokenInternal => Error.Failure(
            code: "Account.RefreshTokenInternal",
            description: "An unexpected error occurred while refreshing the token.");

        /// <summary>
        /// Gets an error indicating an internal failure during confirmation email resend.
        /// </summary>
        public static Error ResendConfirmationInternal => Error.Failure(
            code: "Account.ResendConfirmationInternal",
            description: "An unexpected error occurred while resending the confirmation email.");

        /// <summary>
        /// Gets an error indicating that the social login token is invalid.
        /// </summary>
        public static Error SocialLoginInvalidToken => Error.Unauthorized(
            code: "Account.SocialLoginInvalidToken",
            description: "The social login token is invalid or could not be verified.");

        /// <summary>
        /// Gets an error indicating an internal failure during social login.
        /// </summary>
        public static Error SocialLoginInternal => Error.Failure(
            code: "Account.SocialLoginInternal",
            description: "An unexpected error occurred during social login.");

        /// <summary>
        /// Gets an error indicating an internal failure during account deletion.
        /// </summary>
        public static Error DeletionInternal => Error.Failure(
            code: "Account.DeletionInternal",
            description: "An unexpected error occurred while deleting the account.");

        /// <summary>
        /// Gets an error indicating an internal failure during account retrieval.
        /// </summary>
        public static Error RetrievalInternal => Error.Failure(
            code: "Account.RetrievalInternal",
            description: "An unexpected error occurred while retrieving account details.");

        /// <summary>
        /// Gets an error indicating an internal failure during credential update.
        /// </summary>
        public static Error UpdateCredentialInternal => Error.Failure(
            code: "Account.UpdateCredentialInternal",
            description: "An unexpected error occurred while updating account credentials.");


        /// <summary>
        /// Gets an error indicating the user is not authenticated.
        /// </summary>
        public static Error NotAuthenticated => Error.Unauthorized(
            code: "Account.NotAuthenticated",
            description: "User is not authenticated. Please log in to proceed."
        );
    }
}
