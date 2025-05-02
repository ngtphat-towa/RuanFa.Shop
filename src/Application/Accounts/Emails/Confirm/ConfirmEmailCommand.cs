using ErrorOr;
using MediatR;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Emails.Confirm;
public record ConfirmEmailCommand : ICommand
{
    public required string UserId { get; init; }
    public required string Code { get; init; }
    public string? ChangedEmail { get; init; }
}

public class ConfirmEmailCommandHandler(
    IAccountService accountService)
    : IRequestHandler<ConfirmEmailCommand, ErrorOr<Success>>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<ErrorOr<Success>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var confirmEmailResult = await _accountService.ConfirmEmailAsync(request.UserId,
            request.Code,
            request.ChangedEmail,
            cancellationToken);

        // TODO: add email promotions if email valid and the customer agree to get promotions
        return confirmEmailResult.IsError 
            ? confirmEmailResult.Errors 
            : confirmEmailResult.Value;
    }
}
