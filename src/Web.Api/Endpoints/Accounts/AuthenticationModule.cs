using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Accounts.Authentication.Login;
using RuanFa.Shop.Application.Accounts.Authentication.Register;
using RuanFa.Shop.Application.Accounts.Authentication.RefreshToken;
using RuanFa.Shop.Web.Api.Extensions;
using RuanFa.Shop.Application.Accounts.Models;

namespace RuanFa.Shop.Web.Api.Endpoints.Accounts;

public class AuthenticationModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        // Register
        group.MapPost("/register", Register)
            .WithName("Register")
            .WithDescription("Creates a new user account")
            .WithSummary("Register new user")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Login
        group.MapPost("/login", Login)
            .WithName("Login")
            .WithDescription("Authenticates a user and returns JWT tokens")
            .WithSummary("User login")
            .Produces<TokenResult>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Refresh Token
        group.MapPost("/refresh-token", RefreshToken)
            .WithName("RefreshToken")
            .WithDescription("Refreshes an expired access token using a refresh token")
            .WithSummary("Refresh access token")
            .Produces<TokenResult>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Register(
        RegisterCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> Login(
        LoginQuery query,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }
}
