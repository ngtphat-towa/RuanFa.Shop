using ErrorOr;
using MediatR;
using NSubstitute;
using RuanFa.Shop.Application.Common.Behaviours;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Application.Common.Security.Policies;
using Shouldly;
using Xunit;

namespace RuanFa.Shop.Application.UnitTests.Common.Behaviors;

public class AuthorizationPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_NoAttributes_NotIUserMessage_ProceedsWithoutAuthorization()
    {
        // Arrange
        var request = new NoAttributesRequest();
        ErrorOr<Response> expectedResponse = Response.Instance;

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult(expectedResponse));

        var authService = Substitute.For<IAuthorizationService>();
        var behavior = new AuthorizationPipelineBehavior<NoAttributesRequest, ErrorOr<Response>>(authService);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Response.Instance);
        await authService.DidNotReceive().AuthorizeRequest(
            Arg.Any<List<string>>(),
            Arg.Any<List<string>>(),
            Arg.Any<List<string>>(),
            Arg.Any<CancellationToken>());
        await next.Received(1)();
    }

    [Fact]
    public async Task Handle_WithAttributes_UserAuthorized_ProceedsToNextHandler()
    {
        // Arrange
        var request = new WithAttributesRequest();
        ErrorOr<Response> expectedResponse = Response.Instance;

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult(expectedResponse));

        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeRequest(
            Arg.Is<List<string>>(r => r.SequenceEqual(new[] { "Admin" })),
            Arg.Is<List<string>>(p => p.SequenceEqual(new[] { "Read", "Write" })),
            Arg.Is<List<string>>(pol => pol.SequenceEqual(new[] { "PolicyA" })),
            Arg.Any<CancellationToken>()
        ).Returns(Task.FromResult<ErrorOr<Success>>(new Success()));

        var behavior = new AuthorizationPipelineBehavior<WithAttributesRequest, ErrorOr<Response>>(authService);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Response.Instance);
        await authService.Received(1).AuthorizeRequest(
            Arg.Is<List<string>>(r => r.SequenceEqual(new[] { "Admin" })),
            Arg.Is<List<string>>(p => p.SequenceEqual(new[] { "Read", "Write" })),
            Arg.Is<List<string>>(pol => pol.SequenceEqual(new[] { "PolicyA" })),
            Arg.Any<CancellationToken>());
        await next.Received(1)();
    }

    [Fact]
    public async Task Handle_WithAttributes_UserNotAuthorized_ReturnsError()
    {
        // Arrange
        var request = new WithAttributesRequest();
        var error = Error.Unauthorized("Not authorized");

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult<ErrorOr<Response>>(Response.Instance)); // Mocked for safety, not called

        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeRequest(
            Arg.Any<List<string>>(),
            Arg.Any<List<string>>(),
            Arg.Any<List<string>>(),
            Arg.Any<CancellationToken>()
        ).Returns(Task.FromResult<ErrorOr<Success>>(error));

        var behavior = new AuthorizationPipelineBehavior<WithAttributesRequest, ErrorOr<Response>>(authService);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
        await authService.Received(1).AuthorizeRequest(
            Arg.Any<List<string>>(),
            Arg.Any<List<string>>(),
            Arg.Any<List<string>>(),
            Arg.Any<CancellationToken>());
        await next.DidNotReceive()();
    }

    [Fact]
    public async Task Handle_IUserMessage_NoAttributes_AppliesDefaultPolicy()
    {
        // Arrange
        var request = new UserMessageNoAttributesRequest("user123");
        ErrorOr<Response> expectedResponse = Response.Instance;

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult(expectedResponse));

        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeUserRequest(
            request,
            Arg.Is((List<string>?)null),
            Arg.Is((List<string>?)null),
            Arg.Is<List<string>>(pol => pol.Contains(Policy.SelfOrAdmin)), // Changed to Contains instead of SequenceEqual
            Arg.Any<CancellationToken>()
        ).Returns(Task.FromResult<ErrorOr<Success>>(new Success()));

        var behavior = new AuthorizationPipelineBehavior<UserMessageNoAttributesRequest, ErrorOr<Response>>(authService);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Response.Instance);
        await authService.Received(1).AuthorizeUserRequest(
            request,
            Arg.Is((List<string>?)null),
            Arg.Is((List<string>?)null),
            Arg.Is<List<string>>(pol => pol.Contains(Policy.SelfOrAdmin)),
            Arg.Any<CancellationToken>());
        await next.Received(1)();
    }

    [Fact]
    public async Task Handle_IUserMessage_WithAttributes_UsesSpecifiedAttributes()
    {
        // Arrange
        var request = new UserMessageWithAttributesRequest("user123");
        ErrorOr<Response> expectedResponse = Response.Instance;

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult(expectedResponse));

        var authService = Substitute.For<IAuthorizationService>();
        authService.AuthorizeUserRequest(
            request,
            Arg.Is<List<string>>(r => r.SequenceEqual(new[] { "User" })),
            Arg.Is<List<string>>(p => p.SequenceEqual(new[] { "Read" })),
            Arg.Is((List<string>?)null),
            Arg.Any<CancellationToken>()
        ).Returns(Task.FromResult<ErrorOr<Success>>(new Success()));

        var behavior = new AuthorizationPipelineBehavior<UserMessageWithAttributesRequest, ErrorOr<Response>>(authService);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Response.Instance);
        await authService.Received(1).AuthorizeUserRequest(
            request,
            Arg.Is<List<string>>(r => r.SequenceEqual(new[] { "User" })),
            Arg.Is<List<string>>(p => p.SequenceEqual(new[] { "Read" })),
            Arg.Is((List<string>?)null),
            Arg.Any<CancellationToken>());
        await next.Received(1)();
    }
}
