using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Accounts.Emails.Resend;
using RuanFa.Shop.Application.Accounts.Emails.Confirm;
using RuanFa.Shop.Web.Api.Extensions;

namespace RuanFa.Shop.Web.Api.Endpoints.Accounts;

public class EmailModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account/emails")
            .WithTags("Email Management")
            .WithOpenApi();

        // Confirm Email
        group.MapPost("/confirm", ConfirmEmail)
            .WithName("ConfirmEmail")
            .WithDescription("Confirms a user's email address")
            .WithSummary("Confirm email")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Resend Confirmation Email
        group.MapPost("/resend-confirmation", ResendConfirmationEmail)
            .WithName("ResendConfirmationEmail")
            .WithDescription("Resends the email confirmation link")
            .WithSummary("Resend confirmation email")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

    }

    private static async Task<IResult> ConfirmEmail(
        ConfirmEmailCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> ResendConfirmationEmail(
        ResendConfirmationEmailCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }
}
