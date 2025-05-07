using ErrorOr;

namespace RuanFa.Shop.Domain.Attributes.Errors;

public static partial class DomainErrors
{
    public static class AttributeGroup
    {
        public static Error InvalidId => Error.Validation(
            code: "AttributeGroup.InvalidId",
            description: "The attribute group ID is invalid or empty.");

        public static Error EmptyName => Error.Validation(
            code: "AttributeGroup.EmptyName",
            description: "The attribute group name is required and cannot be empty.");

        public static Error NameTooShort => Error.Validation(
            code: "AttributeGroup.NameTooShort",
            description: "The attribute group name must be at least 3 characters long.");

        public static Error NameTooLong => Error.Validation(
            code: "AttributeGroup.NameTooLong",
            description: "The attribute group name cannot exceed 50 characters.");
    }
}
