using ErrorOr;

namespace RuanFa.Shop.Domain.Attributes.Errors;

public static partial class DomainErrors
{
    public static class AttributeGroupAttribute
    {
        public static Error InvalidGroupId => Error.Validation(
            code: "AttributeGroupAttribute.InvalidGroupId",
            description: "The attribute group ID cannot be empty."
        );

        public static Error InvalidAttributeId => Error.Validation(
            code: "AttributeGroupAttribute.InvalidAttributeId",
            description: "The attribute ID cannot be empty."
        );

        public static Error DuplicateAttribute => Error.Conflict(
            code: "AttributeGroupAttribute.DuplicateAttribute",
            description: "The attribute is already associated with this group."
        );

        public static Error AttributeNotFound => Error.NotFound(
            code: "AttributeGroupAttribute.AttributeNotFound",
            description: "The specified attribute is not associated with this group."
        );
    }
}
