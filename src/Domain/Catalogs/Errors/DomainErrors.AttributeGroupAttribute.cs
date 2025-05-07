using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class AttributeGroupAttribute
    {
        public static Error InvalidGroupId => Error.Validation(
            code: "AttributeGroupAttribute.InvalidGroupId",
            description: "The attribute group ID must be a valid, non-empty GUID."
        );

        public static Error InvalidAttributeId => Error.Validation(
            code: "AttributeGroupAttribute.InvalidAttributeId",
            description: "The attribute ID must be a valid, non-empty GUID."
        );

        public static Error DuplicateAttribute => Error.Conflict(
            code: "AttributeGroupAttribute.DuplicateAttribute",
            description: "The attribute is already associated with this attribute group."
        );

        public static Error AttributeNotFound => Error.NotFound(
            code: "AttributeGroupAttribute.AttributeNotFound",
            description: "The specified attribute is not associated with this attribute group."
        );
    }
}
