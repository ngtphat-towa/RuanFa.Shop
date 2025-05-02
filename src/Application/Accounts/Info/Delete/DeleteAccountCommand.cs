using ErrorOr;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Info.Delete;
public sealed record DeleteAccountCommand : ICommand<AccountInfoResult>;

public class DeleteAccountCommandHandler(IAccountService accountService) 
    : ICommandHandler<DeleteAccountCommand, AccountInfoResult>
{
    public async Task<ErrorOr<AccountInfoResult>> Handle(
        DeleteAccountCommand command,
        CancellationToken cancellationToken)
    {
        return await accountService.DeleteAccountAsync(cancellationToken);
    }
}
