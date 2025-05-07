using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class AttributeOption
    {
        public static Error InvalidAttributeId => Error.Validation(
            code: "AttributeOption.InvalidAttributeId",
            description: "The attribute ID cannot be empty."
        );

        public static Error EmptyAttributeCode => Error.Validation(
            code: "AttributeOption.EmptyAttributeCode",
            description: "The attribute code is required and cannot be empty."
        );

        public static Error AttributeCodeTooShort => Error.Validation(
            code: "AttributeOption.AttributeCodeTooShort",
            description: "The attribute code must be at least 3 characters long."
        );

        public static Error AttributeCodeTooLong => Error.Validation(
            code: "AttributeOption.AttributeCodeTooLong",
            description: "The attribute code cannot exceed 50 characters."
        );

        public static Error InvalidAttributeCodeFormat => Error.Validation(
            code: "AttributeOption.InvalidAttributeCodeFormat",
            description: "The attribute code must contain only alphanumeric characters, dashes, or underscores."
        );

        public static Error EmptyOptionText => Error.Validation(
            code: "AttributeOption.EmptyOptionText",
            description: "The option text is required and cannot be empty."
        );

        public static Error OptionTextTooLong => Error.Validation(
            code: "AttributeOption.OptionTextTooLong",
            description: "The option text cannot exceed 100 characters."
        );
    }
}
