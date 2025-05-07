using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors
{
    public static partial class DomainErrors
    {
        public static class Category
        {
            public static Error InvalidId => Error.Validation(
                code: "Category.InvalidId",
                description: "The category ID must be a valid, non-empty GUID."
            );

            public static Error EmptyName => Error.Validation(
                code: "Category.EmptyName",
                description: "The category name is required and cannot be empty."
            );

            public static Error NameTooShort => Error.Validation(
                code: "Category.NameTooShort",
                description: "The category name must be at least 3 characters long."
            );

            public static Error NameTooLong => Error.Validation(
                code: "Category.NameTooLong",
                description: "The category name must not exceed 100 characters."
            );

            public static Error EmptyUrlKey => Error.Validation(
                code: "Category.EmptyUrlKey",
                description: "The URL key is required and cannot be empty."
            );

            public static Error InvalidUrlKeyFormat => Error.Validation(
                code: "Category.InvalidUrlKeyFormat",
                description: "The URL key must contain only lowercase letters, numbers, and dashes."
            );

            public static Error UrlKeyTooLong => Error.Validation(
                code: "Category.UrlKeyTooLong",
                description: "The URL key must not exceed 255 characters."
            );

            public static Error InvalidImage => Error.Validation(
                code: "Category.InvalidImage",
                description: "The category image must have a valid URL and alt text."
            );

            public static Error InvalidPosition => Error.Validation(
                code: "Category.InvalidPosition",
                description: "The position must be a non-negative number."
            );

            public static Error CircularReference => Error.Conflict(
                code: "Category.CircularReference",
                description: "A category cannot be its own parent or create a circular reference."
            );

            public static Error UrlKeyAlreadyExists => Error.Conflict(
                code: "Category.UrlKeyAlreadyExists",
                description: "The URL key is already in use by another category."
            );

            public static Error ShortDescriptionTooLong => Error.Validation(
                code: "Category.ShortDescriptionTooLong",
                description: "The short description must not exceed 500 characters."
            );

            public static Error DescriptionTooLong => Error.Validation(
                code: "Category.DescriptionTooLong",
                description: "The description must not exceed 2000 characters."
            );

            public static Error MetaTitleTooLong => Error.Validation(
                code: "Category.MetaTitleTooLong",
                description: "The meta title must not exceed 60 characters."
            );

            public static Error MetaKeywordsTooLong => Error.Validation(
                code: "Category.MetaKeywordsTooLong",
                description: "The meta keywords must not exceed 255 characters."
            );

            public static Error MetaDescriptionTooLong => Error.Validation(
                code: "Category.MetaDescriptionTooLong",
                description: "The meta description must not exceed 160 characters."
            );
        }
    }
}
