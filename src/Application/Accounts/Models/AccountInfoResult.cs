namespace RuanFa.Shop.Application.Accounts.Models;

public record AccountInfoResult(
    string UserId,
    string Email,
    string? FullName,
    bool IsEmailVerified,
    DateTime? Created,
    DateTime? LastLogin,
    List<string>? Roles = null,
    List<string>? Permisions = null
);
