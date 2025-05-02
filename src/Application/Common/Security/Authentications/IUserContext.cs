namespace RuanFa.Shop.Application.Common.Security.Authentications;

public interface IUserContext
{
    string? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get;}
}
