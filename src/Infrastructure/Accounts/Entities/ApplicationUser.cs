using Microsoft.AspNetCore.Identity;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Infrastructure.Accounts.Entities;

internal class ApplicationUser : IdentityUser, IAuditable
{
    #region Properties
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool IsActive { get; set; } = true;

    // Adudits
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    #endregion
    #region Relationship
    public Guid? ProfileId { get; set; }
    public UserProfile? Profile { get; set; }
    #endregion
}
