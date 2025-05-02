using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Application.Accounts.Emails.Update;
using RuanFa.Shop.Web.Api.Extensions;
using RuanFa.Shop.Application.Accounts.Info.Delete;
using RuanFa.Shop.Application.Accounts.Info.Get;
using RuanFa.Shop.Application.Accounts.Passwords.Update;

namespace RuanFa.Shop.Web.Api.Endpoints.Accounts
{
    public class AccountManagementModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/account")
                .WithTags("Account Management")
                .WithOpenApi()
                .RequireAuthorization();

            // Get account info
            group.MapGet("/", GetAccountInfo)
                .WithName("GetAccountInfo")
                .WithDescription("Retrieves detailed information about the authenticated user's account")
                .WithSummary("Get account information")
                .Produces<AccountInfoResult>()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            // Update email
            group.MapPut("/email", UpdateEmail)
                .WithName("UpdateAccountEmail")
                .WithDescription("Updates the authenticated user's email address")
                .WithSummary("Update email address")
                .Produces(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            // Update password
            group.MapPut("/password", UpdatePassword)
                .WithName("UpdateAccountPassword")
                .WithDescription("Updates the authenticated user's password")
                .WithSummary("Update password")
                .Produces(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            // Delete account
            group.MapDelete("/", DeleteAccount)
                .WithName("DeleteAccount")
                .WithDescription("Deletes the authenticated user's account and associated profile")
                .WithSummary("Delete account")
                .Produces<AccountInfoResult>()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }

        private static async Task<IResult> GetAccountInfo(
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var query = new GetAccountInfoQuery();
            var result = await sender.Send(query, cancellationToken);
            return result.ToTypedResult();
        }

        private static async Task<IResult> UpdateEmail(
            UpdateEmailCommand command,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToTypedResult();
        }

        private static async Task<IResult> UpdatePassword(
            UpdatePasswordCommand command,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToTypedResult();
        }

        private static async Task<IResult> DeleteAccount(
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var command = new DeleteAccountCommand();
            var result = await sender.Send(command, cancellationToken);
            return result.ToTypedResult();
        }
    }
}
