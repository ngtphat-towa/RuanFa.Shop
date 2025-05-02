using ErrorOr;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Authentication.RefreshToken;

public record RefreshTokenCommand : ICommand<TokenResult>
{
    public string RefreshToken { get; init; } = null!;
}
internal class RefreshTokenCommandHandler(IAccountService accountService) 
    : ICommandHandler<RefreshTokenCommand, TokenResult>
{
    public async Task<ErrorOr<TokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenResult = await accountService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        return refreshTokenResult.IsError
            ? refreshTokenResult.Errors
            : refreshTokenResult.Value;
    }
}
