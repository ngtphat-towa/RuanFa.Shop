namespace RuanFa.Shop.Domain.Catalogs.Enums;

/// <summary>
/// Represents the tax classification for a product.
/// </summary>
public enum TaxClass
{
    /// <summary>
    /// No tax applies to the product.
    /// </summary>
    None = 0,

    /// <summary>
    /// Standard tax rate applies.
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Reduced tax rate applies.
    /// </summary>
    Reduced = 2,

    /// <summary>
    /// Exempt from tax.
    /// </summary>
    Exempt = 3
}
