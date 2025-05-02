using ErrorOr;

namespace RuanFa.Shop.Domain.Accounts.Errors;
public static partial class DomainErrors
{
    public static class EmailConfirmation
    {
        public static Error UserIdRequired => Error.Validation(
            code: "EmailConfirmation.UserIdRequired",
            description: "User ID is required.");

        public static Error InvalidUserIdFormat => Error.Validation(
            code: "EmailConfirmation.InvalidUserIdFormat",
            description: "User ID format is invalid.");

        public static Error ConfirmationCodeRequired => Error.Validation(
            code: "EmailConfirmation.ConfirmationCodeRequired",
            description: "Confirmation code is required.");

        public static Error InvalidConfirmationCodeFormat => Error.Validation(
            code: "EmailConfirmation.InvalidConfirmationCodeFormat",
            description: "Confirmation code format is invalid.");

        public static Error InvalidEmailFormat => Error.Validation(
            code: "EmailConfirmation.InvalidEmailFormat",
            description: "Email format is invalid.");

        public static Error ResendEmailRequired => Error.Validation(
            code: "EmailConfirmation.ResendEmailRequired",
            description: "Email address is required for resending confirmation.");

        public static Error ResendInvalidEmailFormat => Error.Validation(
            code: "EmailConfirmation.ResendInvalidEmailFormat",
            description: "Email address format is invalid for resending confirmation.");

    }
    public static class EmailUpdate
    {
        public static Error NewEmailRequired => Error.Validation(
            code: "EmailUpdate.NewEmailRequired",
            description: "New email address is required.");

        public static Error InvalidEmailFormat => Error.Validation(
            code: "EmailUpdate.InvalidEmailFormat",
            description: "New email format is invalid.");
    }
}
