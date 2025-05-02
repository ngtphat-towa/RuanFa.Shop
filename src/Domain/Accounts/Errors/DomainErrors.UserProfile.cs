using ErrorOr;

namespace RuanFa.Shop.Domain.Accounts.Errors;

public static partial class DomainErrors
{
    public static class UserProfile
    {
        // General User Profile Errors
        public static Error InvalidUserId => Error.Validation(
            code: "UserProfile.InvalidUserId",
            description: "User ID cannot be empty or invalid.");

        public static Error NotFound => Error.NotFound(
            code: "UserProfile.NotFound",
            description: "The user profile was not found.");

        public static Error UnauthorizedAccess => Error.Unauthorized(
            code: "UserProfile.UnauthorizedAccess",
            description: "User does not have permission to access this profile.");

        // Profile Information Errors
        public static Error EmailRequired => Error.Validation(
            code: "UserProfile.EmailRequired",
            description: "Email address is required for user profile.");

        public static Error InvalidEmailFormat => Error.Validation(
            code: "UserProfile.InvalidEmailFormat",
            description: "Email address format is invalid.");

        public static Error FullNameRequired => Error.Validation(
            code: "UserProfile.FullNameRequired",
            description: "Full name is required for user profile.");

        public static Error NameTooShort => Error.Validation(
            code: "UserProfile.NameTooShort",
            description: "User name is too short. It must be at least 2 characters long.");

        // Loyalty and Orders Errors
        public static Error InvalidPoints => Error.Validation(
            code: "UserProfile.InvalidPoints",
            description: "Loyalty points must be zero or greater.");

        public static Error InvalidPointsToRedeem => Error.Validation(
            code: "UserProfile.InvalidPointsToRedeem",
            description: "The points to redeem must be greater than zero and within available balance.");

        public static Error InvalidOrder => Error.Validation(
            code: "UserProfile.InvalidOrder",
            description: "Associated order is invalid or null.");

        public static Error OrderNotFound => Error.NotFound(
            code: "UserProfile.OrderNotFound",
            description: "The specified order was not found in the user profile.");
    }
}
