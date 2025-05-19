using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class AttributeOption
    {
        public static Error InvalidId => Error.Validation(
            code: "AttributeGroup.InvalidId",
            description: "The attribute group ID is invalid or empty.");

        public static Error InvalidAttributeId => Error.Validation(
            code: "AttributeOption.InvalidAttributeId",
            description: "The attribute ID cannot be empty."
        );

        public static Error InvalidCodeFormat => Error.Validation(
            code: "AttributeOption.InvalidCodeFormat",
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

        public static Error NotFound => Error.NotFound(
            code: "AttributeOption.NotFound",
            description: "The specified attribute option was not found."
        );
    }
}
