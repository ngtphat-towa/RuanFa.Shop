using Microsoft.AspNetCore.Http;
using RuanFa.Shop.Application.Common.Security.Authentications;
using System.Security.Claims;

namespace RuanFa.Shop.Infrastructure.Security.Authentication;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid? UserId =>
        httpContextAccessor.HttpContext?.User.GetUserId();

    public string? Username =>
        httpContextAccessor.HttpContext?.User.GetUsername();

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}

internal static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out Guid parsedUserId) ? parsedUserId : null;
    }

    public static string? GetUsername(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.Name);
    }
}
