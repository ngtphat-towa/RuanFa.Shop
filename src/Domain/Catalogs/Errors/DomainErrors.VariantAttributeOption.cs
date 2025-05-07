using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class VariantAttributeOption
    {
        public static Error InvalidVariantId => Error.Validation(
            code: "VariantAttributeOption.InvalidVariantId",
            description: "The variant ID must be a valid, non-empty GUID."
        );

        public static Error InvalidAttributeOptionId => Error.Validation(
            code: "VariantAttributeOption.InvalidAttributeOptionId",
            description: "The attribute option ID must be a valid, non-empty GUID."
        );

        public static Error DuplicateAttributeOption => Error.Conflict(
            code: "VariantAttributeOption.DuplicateAttributeOption",
            description: "The attribute option is already associated with this variant."
        );

        public static Error AttributeOptionNotFound => Error.NotFound(
            code: "VariantAttributeOption.AttributeOptionNotFound",
            description: "The specified attribute option is not associated with this variant."
        );

        public static Error NoAttributeOptionsProvided => Error.Validation(
            code: "VariantAttributeOption.NoAttributeOptionsProvided",
            description: "At least one attribute option must be provided."
        );

        public static Error InvalidAttributeForGroup => Error.Validation(
            code: "VariantAttributeOption.InvalidAttributeForGroup",
            description: "The attribute option does not belong to the product's attribute group."
        );
    }

}
