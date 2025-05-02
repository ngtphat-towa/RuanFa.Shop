using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Orders;

public class Order : Entity<Guid>
{
    #region Relationship
    public string? UserId { get; set; }
    public UserProfile? Profile { get; set; }
    #endregion
}
