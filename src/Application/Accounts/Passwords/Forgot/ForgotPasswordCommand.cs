using ErrorOr;
using MediatR;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Passwords.Forgot;
public record ForgotPasswordCommand : ICommand
{
    public required string Email { get; init; }
}
public class ForgotPasswordCommandHandler(IAccountService identityService)
    : IRequestHandler<ForgotPasswordCommand, ErrorOr<Success>>
{
    private readonly IAccountService _identityService = identityService;

    public async Task<ErrorOr<Success>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var forgetPasswordResult = await _identityService.ForgotPasswordAsync(request.Email, cancellationToken);

        return forgetPasswordResult.IsError
            ? forgetPasswordResult.Errors
            : forgetPasswordResult.Value;
    }
}
