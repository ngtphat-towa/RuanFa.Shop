using ErrorOr;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Passwords.Update;

public sealed class UpdatePasswordCommand : ICommand<Updated>
{
    public string? NewPassword { get; init; }
    public string? OldPassword { get; init; }
}

public class UpdatePasswordCommandHandler(IAccountService idenittyService) : ICommand<UpdatePasswordCommand>
{
    private readonly IAccountService _idenittyService = idenittyService;

    public async Task<ErrorOr<Updated>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _idenittyService.UpdateAccountCredentialAsync(
            null,
            request.OldPassword,
            request.NewPassword,
            cancellationToken);

    }
}
