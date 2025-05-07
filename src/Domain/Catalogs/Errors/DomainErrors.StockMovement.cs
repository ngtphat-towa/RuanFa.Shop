using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class StockMovement
    {
        public static Error NotFound =>
            Error.NotFound(
                "StockMovement.NotFound",
                "The specified stock movement was not found.");

        public static Error InvalidVariantId => Error.Validation(
            code: "StockMovement.InvalidVariantId",
            description: "Variant ID cannot be empty.");

        public static Error InvalidQuantity => Error.Validation(
            code: "StockMovement.InvalidQuantity",
            description: "Quantity must be greater than zero.");

        public static Error InvalidReferenceId => Error.Validation(
            code: "StockMovement.InvalidReferenceId",
            description: "Reference ID cannot be empty when provided.");

        public static Error InvalidNotes => Error.Validation(
            code: "StockMovement.InvalidNotes",
            description: "Notes cannot be empty when provided.");

        public static Error InvalidMovementType => Error.Validation(
            code: "StockMovement.InvalidMovementType",
            description: "The specified movement type is invalid for this operation.");

    }

}


