using ErrorOr;

namespace RuanFa.Shop.Domain.Attributes.Errors;

public static partial class DomainErrors
{
    public static class CatalogAttribute
    {
        public static Error InvalidAttributeId => Error.Validation(
            code: "CatalogAttribute.InvalidAttributeId",
            description: "The specified Catalog Attribute ID is invalid.");

        public static Error InvalidCode => Error.Validation(
            code: "CatalogAttribute.InvalidCode",
            description: "The attribute code must not be empty or whitespace.");

        public static Error CodeTooLong => Error.Validation(
            code: "CatalogAttribute.CodeTooLong",
            description: "The attribute code must not exceed 50 characters.");

        public static Error InvalidName => Error.Validation(
            code: "CatalogAttribute.InvalidName",
            description: "The attribute name must not be empty or whitespace.");

        public static Error NameTooLong => Error.Validation(
            code: "CatalogAttribute.NameTooLong",
            description: "The attribute name must not exceed 100 characters.");

        public static Error InvalidType => Error.Validation(
            code: "CatalogAttribute.InvalidType",
            description: "The specified attribute type is invalid.");

        public static Error DuplicateCode(string code) => Error.Validation(
            code: "CatalogAttribute.DuplicateCode",
            description: $"A catalog attribute with the code '{code}' already exists.");

        public static Error NotFound => Error.NotFound(
            code: "CatalogAttribute.NotFound",
            description: "The specified catalog attribute was not found.");

        public static Error InvalidSortOrder => Error.Validation(
            code: "CatalogAttribute.InvalidSortOrder",
            description: "Sort order must be a non-negative number.");
    }
}
