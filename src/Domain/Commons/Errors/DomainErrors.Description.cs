using ErrorOr;

namespace RuanFa.Shop.Domain.Commons.Errors;

public static partial class DomainErrors
{
    public static class Description
    {
        public static Error InvalidType => Error.Validation(
            "Description.InvalidType",
            "The description type is not valid."
        );

        public static Error EmptyValue => Error.Validation(
            "Description.EmptyValue",
            "The description content cannot be empty."
        );

        public static Error ValueTooLong => Error.Validation(
            "Description.ValueTooLong",
            "The description content cannot exceed 4000 characters."
        );
    }
}
