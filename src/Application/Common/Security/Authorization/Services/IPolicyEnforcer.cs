using ErrorOr;
using RuanFa.Shop.Application.Common.Security.Authorization.Models;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Common.Security.Authorization.Services;

public interface IPolicyEnforcer
{
    ErrorOr<Success> Authorize(
        IUserMessage? request, 
        CurrentUser? user, 
        string policy);
}
