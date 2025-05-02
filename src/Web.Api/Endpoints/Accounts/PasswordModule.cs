using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Accounts.Passwords.Forgot;
using RuanFa.Shop.Application.Accounts.Passwords.Reset;
using RuanFa.Shop.Application.Accounts.Passwords.Update;
using RuanFa.Shop.Web.Api.Extensions;

namespace RuanFa.Shop.Web.Api.Endpoints.Accounts;

public class PasswordModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account/passwords")
            .WithTags("Password Management")
            .WithOpenApi();

        // Forgot Password
        group.MapPost("/forgot", ForgotPassword)
            .WithName("ForgotPassword")
            .WithDescription("Initiates password reset process")
            .WithSummary("Forgot password")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Reset Password
        group.MapPost("/reset", ResetPassword)
            .WithName("ResetPassword")
            .WithDescription("Resets user password using a valid token")
            .WithSummary("Reset password")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> ForgotPassword(
        ForgotPasswordCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> ResetPassword(
        ResetPasswordCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }
}
