namespace RuanFa.Shop.Application.Accounts.Models;

public record RoleDetailResult : RoleResult
{
    public List<string>? Permissions { get; set; }
}
