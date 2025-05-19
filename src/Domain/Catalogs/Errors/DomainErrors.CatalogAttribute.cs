using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class CatalogAttribute
    {
        public static Error InvalidId => Error.Validation(
            code: "CatalogAttribute.InvalidId",
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

        public static Error NotFound(List<Guid>? missingIds = null) => Error.NotFound(
            code: "CatalogAttribute.NotFound",
            description: missingIds?.Any() == true
                ? $"One or more catalog attributes were not found: {string.Join(", ", missingIds)}."
                : "The specified catalog attribute was not found."
        );

        public static Error InvalidSortOrder => Error.Validation(
            code: "CatalogAttribute.InvalidSortOrder",
            description: "The sort order must be a non-negative number."
        );

        public static Error InUse => Error.Validation(
            code: "CatalogAttribute.InUsed",
            description: "Cannot delete attribute as it's currently in use by one or more attribute groups or products.");
    }
}
