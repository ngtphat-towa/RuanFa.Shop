using ErrorOr;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Emails.Update;

public record UpdateEmailCommand : ICommand<Updated>
{
    public string? NewEmail { get; init; }
}

internal class UpdateEmailCommandHandler(IAccountService accountService)
    : ICommandHandler<UpdateEmailCommand, Updated>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateEmailCommand request, CancellationToken cancellationToken)
    {
        var updateAccountResult = await accountService.UpdateAccountCredentialAsync(request.NewEmail, null, null, cancellationToken);

        return updateAccountResult.IsError
            ? updateAccountResult.Errors
            : updateAccountResult.Value;
    }
}
