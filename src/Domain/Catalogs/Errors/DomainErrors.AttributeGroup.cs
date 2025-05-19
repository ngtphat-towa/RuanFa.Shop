using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

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

        public static Error DuplicateName(string name) => Error.Conflict(
              code: "AttributeGroup.DuplicateName",
              description: $"An attribute group with the name '{name}' already exists.");

        public static Error NameTooShort => Error.Validation(
            code: "AttributeGroup.NameTooShort",
            description: "The attribute group name must be at least 3 characters long."
        );

        public static Error NameTooLong => Error.Validation(
            code: "AttributeGroup.NameTooLong",
            description: "The attribute group name must not exceed 50 characters."
        );

        public static Error NotFound(List<Guid>? missingIds = null) => Error.NotFound(
            code: "AttributeGroup.NotFound",
            description: missingIds?.Any() == true
                ? $"One or more attribute groups were not found: {string.Join(", ", missingIds)}."
                : "The specified attribute group was not found.");

        public static Error InUse => Error.Validation(
            code: "AttributeGroup.InUse",
            description: "Cannot delete the attribute group because it is still associated with one or more attributes or products."
        );

    }
}
