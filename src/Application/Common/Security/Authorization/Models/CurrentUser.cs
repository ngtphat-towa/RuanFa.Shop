namespace RuanFa.Shop.Application.Common.Security.Authorization.Models;

public record CurrentUser(
    string UserId,
    string Email,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> Roles);
