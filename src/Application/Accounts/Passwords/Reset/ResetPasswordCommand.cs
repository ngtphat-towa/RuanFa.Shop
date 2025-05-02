using ErrorOr;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Passwords.Reset;
public record ResetPasswordCommand : ICommand
{
    public required string Email { get; init; }
    public required string ResetToken { get; init; }
    public required string NewPassword { get; init; }
}

internal sealed class ResetPasswordCommandHandler(IAccountService accountService)
: ICommandHandler<ResetPasswordCommand>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<ErrorOr<Success>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.ResetPasswordAsync(
            request.Email,
            request.ResetToken,
            request.NewPassword,
            cancellationToken);
    }
}
