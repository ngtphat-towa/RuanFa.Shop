using ErrorOr;

namespace RuanFa.Shop.Domain.Accounts.Errors;

public static partial class DomainErrors
{
    public static class Account
    {
        // General Validation Errors
        public static Error RequiredField(string field) => Error.Validation(
            code: $"Account.{field}.Required",
            description: $"{field} is required.");

        public static Error InvalidFieldFormat(string field) => Error.Validation(
            code: $"Account.{field}.InvalidFormat",
            description: $"The {field} format is invalid.");

        public static Error FieldTooShort(string field, int minLength) => Error.Validation(
            code: $"Account.{field}.TooShort",
            description: $"{field} must be at least {minLength} characters long.");

        public static Error InvalidPasswordFormat => Error.Validation(
            code: "Account.InvalidPasswordFormat",
            description: "Password must contain at least one number, one letter, and one special character.");

        // Email Errors
        public static Error EmailRequired => RequiredField("Email");
        public static Error InvalidEmailFormat => InvalidFieldFormat("Email");

        // Username Errors
        public static Error UsernameRequired => RequiredField("Username");
        public static Error InvalidUsernameFormat => InvalidFieldFormat("Username");

        // Password Errors
        public static Error PasswordRequired => RequiredField("Password");
        public static Error PasswordTooShort => FieldTooShort("Password", 6);

        // FullName Errors
        public static Error FullNameRequired => RequiredField("FullName");
        public static Error FullNameTooShort => FieldTooShort("FullName", 3);

        // PhoneNumber Errors
        public static Error InvalidPhoneNumber => InvalidFieldFormat("PhoneNumber");

        // Gender Errors
        public static Error GenderRequired => RequiredField("Gender");

        // DateOfBirth Errors
        public static Error InvalidDateOfBirth => Error.Validation(
            code: "Account.InvalidDateOfBirth",
            description: "Date of birth must be a valid date, and the user must be over 18 years old.");
    }
}
