namespace RuanFa.Shop.Domain.Catalogs.Enums;

public enum MovementType
{
    Purchase,   // Stock acquired through purchasing
    Sale,       // Stock sold to customers
    Return,     // Stock returned by customers
    Waste,      // Stock lost due to waste/damage
    Lost,       // Stock lost due to theft/unknown reasons
    Adjustment, // Manual stock adjustments (can be positive or negative)
    Initial     // Initial stock setup
}
