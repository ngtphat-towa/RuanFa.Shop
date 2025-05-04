namespace RuanFa.Shop.Application.Accounts.Models;

public record AccountInfoResult(
    Guid UserId,
    string Email,
    string? FullName,
    bool IsEmailVerified,
    DateTimeOffset? Created,
    DateTimeOffset? LastLogin,
    List<string>? Roles = null,
    List<string>? Permisions = null
);
