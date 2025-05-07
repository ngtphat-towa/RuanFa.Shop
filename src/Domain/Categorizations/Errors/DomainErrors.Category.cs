using ErrorOr;

namespace RuanFa.Shop.Domain.Categorizations.Errors
{
    public static partial class DomainErrors
    {
        public static class Category
        {
            // Error when category name is empty
            public static Error EmptyName => Error.Validation(
                code: "Category.EmptyName",
                description: "The category name is required and cannot be empty."
            );

            // Error when category name is too short
            public static Error NameTooShort => Error.Validation(
                code: "Category.NameTooShort",
                description: "The category name must be at least 3 characters long."
            );

            // Error when category URL key is empty
            public static Error EmptyUrlKey => Error.Validation(
                code: "Category.EmptyUrlKey",
                description: "The URL key is required and cannot be empty."
            );

            // Error when category URL key is invalid
            public static Error InvalidUrlKey => Error.Validation(
                code: "Category.InvalidUrlKey",
                description: "The URL key contains invalid characters or format."
            );

            // Error when category image is invalid
            public static Error InvalidImage => Error.Validation(
                code: "Category.InvalidImage",
                description: "The category image is invalid. Please provide a valid URL and alt text."
            );

            // Error when category status is invalid (must be a valid status, like active/inactive)
            public static Error InvalidStatus => Error.Validation(
                code: "Category.InvalidStatus",
                description: "The category status must be either active or inactive."
            );

            // Error when the parentId is invalid (if it's set, must reference an existing category)
            public static Error InvalidParentId => Error.Validation(
                code: "Category.InvalidParentId",
                description: "The parent category ID is invalid. It must refer to an existing category."
            );

            // Error when the position is invalid (must be a valid number or within a specific range)
            public static Error InvalidPosition => Error.Validation(
                code: "Category.InvalidPosition",
                description: "The category position is invalid. It must be a valid small integer."
            );

            // Error when the 'includeInNav' field is set to an invalid value
            public static Error InvalidIncludeInNav => Error.Validation(
                code: "Category.InvalidIncludeInNav",
                description: "The 'Include in Navigation' field must be a boolean (true or false)."
            );

            // Error when the category has a circular reference (parent category can't be the same as itself)
            public static Error CircularReference => Error.Validation(
                code: "Category.CircularReference",
                description: "A category cannot be its own parent. Please ensure no circular reference exists."
            );

            // Error when the URL key already exists
            public static Error UrlKeyAlreadyExists => Error.Validation(
                code: "Category.UrlKeyAlreadyExists",
                description: "The URL key already exists. Please choose a unique URL key for this category."
            );

            // Error when category description is too long (max length, adjust as needed)
            public static Error DescriptionTooLong => Error.Validation(
                code: "Category.DescriptionTooLong",
                description: "The category description exceeds the maximum allowed length."
            );

            // Error when the meta title is too long (max length, adjust as needed)
            public static Error MetaTitleTooLong => Error.Validation(
                code: "Category.MetaTitleTooLong",
                description: "The meta title exceeds the maximum allowed length."
            );

            // Error when meta description exceeds the maximum length (for SEO purposes)
            public static Error MetaDescriptionTooLong => Error.Validation(
                code: "Category.MetaDescriptionTooLong",
                description: "The meta description exceeds the maximum allowed length."
            );
        }
    }
}
