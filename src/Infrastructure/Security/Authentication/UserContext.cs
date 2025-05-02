using Microsoft.AspNetCore.Http;
using RuanFa.Shop.Application.Common.Security.Authentications;
using System.Security.Claims;

namespace RuanFa.Shop.Infrastructure.Security.Authentication;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Username => httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
