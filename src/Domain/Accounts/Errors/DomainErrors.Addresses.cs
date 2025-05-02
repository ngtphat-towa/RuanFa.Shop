using ErrorOr;

namespace RuanFa.Shop.Domain.Accounts.Errors;

public static partial class DomainErrors
{
    public static class Address
    {
        // Address Collection Errors
        public static Error AddressesRequired => Error.Validation(
            code: "Address.AddressesRequired",
            description: "At least one address is required.");

        // Address Information Errors
        public static Error AddressLine1Required => Error.Validation(
            code: "Address.AddressLine1Required",
            description: "Address Line 1 is required.");

        public static Error CityRequired => Error.Validation(
            code: "Address.CityRequired",
            description: "City is required.");

        public static Error StateRequired => Error.Validation(
            code: "Address.StateRequired",
            description: "State is required.");

        public static Error CountryRequired => Error.Validation(
            code: "Address.CountryRequired",
            description: "Country is required.");

        public static Error PostalCodeRequired => Error.Validation(
            code: "Address.PostalCodeRequired",
            description: "Postal code is required.");

        public static Error InvalidPostalCodeFormat => Error.Validation(
            code: "Address.InvalidPostalCodeFormat",
            description: "Postal code format is invalid.");
    }

    public static class UserAddress
    {
        // UserAddress Validation Errors
        public static Error AddressTypeRequired => Error.Validation(
            code: "UserAddress.AddressTypeRequired",
            description: "Address type is required.");

        public static Error DefaultAddressFlagInvalid => Error.Validation(
            code: "UserAddress.DefaultAddressFlagInvalid",
            description: "IsDefault flag must be a boolean value (true or false).");
    }
}
