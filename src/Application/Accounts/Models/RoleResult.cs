namespace RuanFa.Shop.Application.Accounts.Models;

public record RoleResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime ? UpdateAt { get; set; }
    public string? UpdateBy { get; set; }
}
