using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Domain.Catalogs.Extensions;

public static class StockMovementNoteFormatter
{
    private static readonly Dictionary<MovementType, string> SourceMappings = new()
    {
        { MovementType.Initial, "INITIAL" },
        { MovementType.Purchase, "SUPPLIER" },
        { MovementType.Sale, "SALES_CHANNEL" },
        { MovementType.Return, "CUSTOMER_SERVICE" },
        { MovementType.Adjustment, "INVENTORY_MANAGEMENT" },
        { MovementType.Waste, "INVENTORY_LOSS" },
        { MovementType.Lost, "INVENTORY_LOSS" }
    };

    public static ErrorOr<string> FormatNote(
        MovementType movementType,
        int quantity,
        Guid? referenceId = null,
        string? additionalContext = null,
        string? createBy = "System")
    {
        // Validate movementType
        if (!SourceMappings.ContainsKey(movementType))
        {
            return DomainErrors.StockMovement.InvalidMovementType;
        }

        // Validate createBy
        if (string.IsNullOrWhiteSpace(createBy))
        {
            return DomainErrors.StockMovement.InvalidNotes;
        }

        var source = SourceMappings[movementType];
        var formattedReferenceId = referenceId.HasValue ? referenceId.ToString() : "N/A";
        var formattedCreateBy = SanitizeCreateBy(createBy);
        var formattedContext = SanitizeContext(additionalContext);
        var formattedQuantity = movementType == MovementType.Adjustment
            ? $"{(quantity >= 0 ? "+" : "")}{quantity}"
            : quantity.ToString();

        return $"{movementType}:{source} | " +
               $"Qty:{formattedQuantity} | " +
               $"RefID:{formattedReferenceId} | " +
               $"User:{formattedCreateBy} | " +
               $"Context:{formattedContext} | " +
               $"Timestamp:{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}";
    }

    private static string SanitizeContext(string? context)
    {
        if (string.IsNullOrWhiteSpace(context))
            return "N/A";

        const int maxLength = 500;
        var trimmed = context.Trim();
        return trimmed.Length > maxLength
            ? trimmed.Substring(0, maxLength - 3) + "..."
            : trimmed;
    }

    private static string SanitizeCreateBy(string createBy)
    {
        const int maxLength = 50;
        var trimmed = createBy.Trim();
        return trimmed.Length > maxLength
            ? trimmed.Substring(0, maxLength - 3) + "..."
            : trimmed;
    }
}
