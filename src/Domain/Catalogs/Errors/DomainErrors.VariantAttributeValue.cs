using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class VariantAttributeValue
    {
        public static Error InvalidVariantId => Error.Validation(
            code: "VariantAttributeValue.InvalidVariantId",
            description: "The variant ID must be a valid, non-empty GUID."
        );

        public static Error InvalidAttributeId => Error.Validation(
            code: "VariantAttributeValue.InvalidAttributeId",
            description: "The attribute ID must be a valid, non-empty GUID."
        );

        public static Error InvalidAttributeOptionId => Error.Validation(
            code: "VariantAttributeValue.InvalidAttributeOptionId",
            description: "The attribute option ID must be a valid, non-empty GUID."
        );


        public static Error NotFound => Error.NotFound(
            code: "VariantAttributeValue.NotFound",
            description: "The specified variant attribute value was not found."
        );

        public static Error InvalidAttributeOptionForAttribute => Error.Validation(
            code: "VariantAttributeValue.InvalidAttributeOptionForAttribute",
            description: "The attribute option does not belong to the specified attribute."
        );

        public static Error InvalidValue => Error.Validation(
            code: "VariantAttributeValue.InvalidValue",
            description: "The value cannot be empty for non-option-based attributes (Text, Number, Boolean, DateTime, Decimal)."
        );

        public static Error ValueNotSupportedForOptionType => Error.Validation(
            code: "VariantAttributeValue.ValueNotSupportedForOptionType",
            description: "A value is not supported for option-based attribute types (Dropdown, Select, Swatch)."
        );

        public static Error OptionNotSupportedForValueType => Error.Validation(
            code: "VariantAttributeValue.OptionNotSupportedForValueType",
            description: "An attribute option is not supported for value-based attribute types (Text, Number, Boolean, DateTime, Decimal)."
        );

        public static Error InvalidAttibuteType => Error.Conflict(
            code: "VariantAttributeValue.InvalidAttibuteType",
            description: "The spetfied attribute type provided was not matching with with current attibute."
        );

        public static Error InvalidNumberFormat => Error.Validation(
            code: "VariantAttributeValue.InvalidNumberFormat",
            description: "The value must be a valid integer for Number attribute type."
        );

        public static Error InvalidBooleanFormat => Error.Validation(
            code: "VariantAttributeValue.InvalidBooleanFormat",
            description: "The value must be a valid boolean (true/false) for Boolean attribute type."
        );

        public static Error InvalidDateTimeFormat => Error.Validation(
            code: "VariantAttributeValue.InvalidDateTimeFormat",
            description: "The value must be a valid date-time for DateTime attribute type."
        );

        public static Error InvalidDecimalFormat => Error.Validation(
            code: "VariantAttributeValue.InvalidDecimalFormat",
            description: "The value must be a valid decimal for Decimal attribute type."
        );

        public static Error InvalidAttributeType => Error.Validation(
            code: "VariantAttributeValue.InvalidAttributeType",
            description: "The attribute type is invalid or not supported."
        );

        public static Error NoAttributeValuesProvided => Error.Validation(
            code: "VariantAttributeValue.NoAttributeValuesProvided",
            description: "At least one attribute value must be provided."
        );

        public static Error DuplicateAttributeValue => Error.Conflict(
            code: "VariantAttributeValue.DuplicateAttributeValue",
            description: "A value for this attribute is already assigned to the variant."
        );

        public static Error InvalidAttributeForGroup => Error.Validation(
            code: "VariantAttributeValue.InvalidAttributeForGroup",
            description: "The attribute does not belong to the product's attribute group."
        );

        public static Error MissingRequiredAttribute(Guid attributeId) => Error.Validation(
            code: "VariantAttributeValue.MissingRequiredAttribute",
            description: $"The required attribute with ID {attributeId} is missing."
        );

        public static Error CannotRemoveRequiredAttribute => Error.Conflict(
            code: "VariantAttributeValue.CannotRemoveRequiredAttribute",
            description: "Cannot remove or clear a required attribute value."
        );
    }
}
