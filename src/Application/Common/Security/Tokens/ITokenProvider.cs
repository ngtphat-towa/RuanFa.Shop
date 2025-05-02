using System.Security.Claims;

namespace RuanFa.Shop.Application.Common.Security.Tokens;

public interface ITokenProvider
{
    string CreateAccessToken(string id, string? email);
    string CreateRefreshToken(string? id);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
