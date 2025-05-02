using ErrorOr;

namespace RuanFa.Shop.Domain.Accounts.Errors;
public  static partial class DomainErrors
{
    public static class Authentication
    {
        public static Error InvalidRefreshToken => Error.Validation(
            code: "Authentication.InvalidRefreshToken",
            description: "Refresh token is invalid or expired.");

        public static Error RefreshTokenRequired => Error.Validation(
            code: "Authentication.RefreshTokenRequired",
            description: "Refresh token is required.");
    }
}
