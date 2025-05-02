using ErrorOr;

namespace RuanFa.Shop.Domain.Accounts.Errors;
public static partial class DomainErrors
{
    public static class PasswordRecovery
    {
        public static Error EmailRequired => Error.Validation(
            code: "PasswordRecovery.EmailRequired",
            description: "Email address is required for password recovery.");

        public static Error InvalidEmailFormat => Error.Validation(
            code: "PasswordRecovery.InvalidEmailFormat",
            description: "Email format is invalid for password recovery.");
    }
    public static class PasswordReset
    {
        public static Error EmailRequired => Error.Validation(
            code: "PasswordReset.EmailRequired",
            description: "Email address is required for password reset.");

        public static Error InvalidEmailFormat => Error.Validation(
            code: "PasswordReset.InvalidEmailFormat",
            description: "Email format is invalid for password reset.");

        public static Error ResetTokenRequired => Error.Validation(
            code: "PasswordReset.ResetTokenRequired",
            description: "Reset token is required.");

        public static Error InvalidResetTokenFormat => Error.Validation(
            code: "PasswordReset.InvalidResetTokenFormat",
            description: "Reset token format is invalid.");

        public static Error NewPasswordRequired => Error.Validation(
            code: "PasswordReset.NewPasswordRequired",
            description: "New password is required.");

        public static Error NewPasswordTooShort => Error.Validation(
            code: "PasswordReset.NewPasswordTooShort",
            description: "New password must be at least 6 characters long.");

        public static Error InvalidPasswordFormat => Error.Validation(
            code: "PasswordReset.InvalidPasswordFormat",
            description: "Password must contain at least one letter, one number, and one special character.");
    }

    public static class PasswordUpdate
    {
        public static Error NewPasswordRequired => Error.Validation(
            code: "PasswordUpdate.NewPasswordRequired",
            description: "New password is required.");

        public static Error OldPasswordRequired => Error.Validation(
            code: "PasswordUpdate.OldPasswordRequired",
            description: "Current password is required.");

        public static Error NewPasswordTooShort => Error.Validation(
            code: "PasswordUpdate.NewPasswordTooShort",
            description: "New password must be at least 6 characters long.");

        public static Error InvalidPasswordFormat => Error.Validation(
            code: "PasswordUpdate.InvalidPasswordFormat",
            description: "Password must contain at least one letter, one number, and one special character.");

        public static Error PasswordsCannotMatch => Error.Validation(
            code: "PasswordUpdate.PasswordsCannotMatch",
            description: "New password cannot be the same as current password.");
    }
}
