using ErrorOr;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Info.Get;

public sealed record GetAccountInfoQuery : IQuery<AccountInfoResult>;
internal sealed class GetAccountInfoEmailQueryHandler(
    IAccountService accountService)
        : IQueryHandler<GetAccountInfoQuery, AccountInfoResult>
{

    public async Task<ErrorOr<AccountInfoResult>> Handle(
        GetAccountInfoQuery request,
        CancellationToken cancellationToken)
    {
        var accountResult = await accountService
            .GetAccountInfoAsync(cancellationToken);

        return accountResult.IsError
            ? accountResult.Errors
            : accountResult.Value   ;
    }
}
