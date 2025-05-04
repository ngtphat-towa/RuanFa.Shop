namespace RuanFa.Shop.Application.Common.Security.Authentications;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get;}
}
