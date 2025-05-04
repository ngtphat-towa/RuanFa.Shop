using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;

namespace RuanFa.Shop.Tests.Shared.Security;
public class TestCurrentUserProvider : ICurrentUserProvider
{
    private CurrentUser? _currentUser;

    public void SetReturn(CurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _currentUser ?? CurrentUserFactory.CreateUser();
    }
}
