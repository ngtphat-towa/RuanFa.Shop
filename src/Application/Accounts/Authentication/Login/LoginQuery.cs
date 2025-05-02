using ErrorOr;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Authentication.Login;

public class LoginQuery : IQuery<TokenResult>
{
    public string UserIdentifier { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
internal sealed class LoginWithEmailQueryHandler(IAccountService accountService)
    : IQueryHandler<LoginQuery, TokenResult>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<ErrorOr<TokenResult>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.AuthenticateAsync(
            request.UserIdentifier, 
            request.Password, 
            cancellationToken);

        return result.IsError 
            ? result.Errors 
            : result.Value;
    }
}
