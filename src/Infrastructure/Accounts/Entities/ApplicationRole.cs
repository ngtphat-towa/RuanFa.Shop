using Microsoft.AspNetCore.Identity;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Infrastructure.Accounts.Entities;

public class ApplicationRole : IdentityRole<Guid>, IAuditable
{
    public ApplicationRole()
    {
    }
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
