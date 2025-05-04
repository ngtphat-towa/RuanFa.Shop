using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Tests.Shared.Constants;

namespace RuanFa.Shop.Tests.Shared.Security;
public class TestUserContext : IUserContext
{
    private Guid? _userId = null;
    private string? _email = null;
    public void SetUserContext(Guid? userId, string? email)
    {
        _userId = userId ?? DataConstants.Accounts.UserId;
        _email = email ?? DataConstants.Accounts.UserEmail;
    }
    public Guid? UserId => _userId ?? DataConstants.Accounts.UserId;
    public string? Username => _email ?? DataConstants.Accounts.UserEmail;

    public bool IsAuthenticated => _userId != null;

}
