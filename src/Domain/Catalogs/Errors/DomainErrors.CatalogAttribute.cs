using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class CatalogAttribute
    {
        public static Error InvalidAttributeId => Error.Validation(
            code: "CatalogAttribute.InvalidAttributeId",
            description: "The catalog attribute ID must be a valid, non-empty GUID."
        );

        public static Error EmptyCode => Error.Validation(
            code: "CatalogAttribute.EmptyCode",
            description: "The attribute code is required and cannot be empty."
        );

        public static Error CodeTooShort => Error.Validation(
            code: "CatalogAttribute.CodeTooShort",
            description: "The attribute code must be at least 3 characters long."
        );

        public static Error CodeTooLong => Error.Validation(
            code: "CatalogAttribute.CodeTooLong",
            description: "The attribute code must not exceed 50 characters."
        );

        public static Error InvalidCodeFormat => Error.Validation(
            code: "CatalogAttribute.InvalidCodeFormat",
            description: "The attribute code must contain only alphanumeric characters, dashes, or underscores."
        );

        public static Error EmptyName => Error.Validation(
            code: "CatalogAttribute.EmptyName",
            description: "The attribute name is required and cannot be empty."
        );

        public static Error NameTooLong => Error.Validation(
            code: "CatalogAttribute.NameTooLong",
            description: "The attribute name must not exceed 100 characters."
        );

        public static Error InvalidType => Error.Validation(
            code: "CatalogAttribute.InvalidType",
            description: "The attribute type must be a valid, non-none type."
        );

        public static Error TypeRequiresOptions => Error.Validation(
            code: "CatalogAttribute.TypeRequiresOptions",
            description: "Dropdown or Swatch attribute types require at least one option."
        );

        public static Error OptionsNotSupportedForType => Error.Validation(
            code: "CatalogAttribute.OptionsNotSupportedForType",
            description: "Options are only supported for Dropdown or Swatch attribute types."
        );

        public static Error DuplicateCode(string code) => Error.Conflict(
            code: "CatalogAttribute.DuplicateCode",
            description: $"An attribute with the code '{code}' already exists."
        );

        public static Error NotFound => Error.NotFound(
            code: "CatalogAttribute.NotFound",
            description: "The specified catalog attribute was not found."
        );

        public static Error OptionNotFound => Error.NotFound(
            code: "CatalogAttribute.OptionNotFound",
            description: "The specified attribute option was not found."
        );

        public static Error InvalidSortOrder => Error.Validation(
            code: "CatalogAttribute.InvalidSortOrder",
            description: "The sort order must be a non-negative number."
        );
    }
}
