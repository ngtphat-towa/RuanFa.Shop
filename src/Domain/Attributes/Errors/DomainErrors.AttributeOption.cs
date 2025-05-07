using ErrorOr;

namespace RuanFa.Shop.Domain.Attributes.Errors;

public static partial class DomainErrors
{
    public static class AttributeOption
    {
        public static Error InvalidId => Error.Validation(
            code: "AttributeOption.InvalidId",
            description: "The attribute option ID is invalid or empty.");

        public static Error EmptyText => Error.Validation(
            code: "AttributeOption.EmptyText",
            description: "The option text is required and cannot be empty.");

        public static Error InvalidAttributeId => Error.Validation(
            code: "AttributeOption.InvalidAttributeId",
            description: "The related attribute ID is invalid or empty.");

        public static Error InvalidAttributeCode => Error.Validation(
            code: "AttributeOption.InvalidAttributeCode",
            description: "The attribute code is required and cannot be empty.");

        public static Error DuplicateOption => Error.Conflict(
            code: "AttributeOption.DuplicateOption",
            description: "An option with the same value already exists for this attribute.");
    }
}
