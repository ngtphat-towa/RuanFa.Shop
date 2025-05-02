using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Commons.Entities;

public class State : Entity<int>
{
    #region Properties
    public required string Name { get; set; } 
    public required string Code { get; set; }
    #endregion

    #region Relationship
    public int CountryId { get; set; }
    public Country Country { get; set; } = default!;
    #endregion
}
