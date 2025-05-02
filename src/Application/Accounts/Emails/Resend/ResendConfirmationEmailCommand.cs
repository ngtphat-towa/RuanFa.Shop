using ErrorOr;
using MediatR;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Emails.Resend;
public record ResendConfirmationEmailCommand : ICommand
{
    public required string Email { get; init; }
}
internal sealed class ResendConfirmationEmailCommandHandler(IAccountService identityService)
        : IRequestHandler<ResendConfirmationEmailCommand, ErrorOr<Success>>
{
    private readonly IAccountService _identityService = identityService;

    public async Task<ErrorOr<Success>> Handle(
        ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ResendConfirmationEmailAsync(request.Email, cancellationToken);
    }
}
