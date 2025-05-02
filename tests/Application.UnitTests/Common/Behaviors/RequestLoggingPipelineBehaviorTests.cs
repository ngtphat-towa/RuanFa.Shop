using ErrorOr;
using MediatR;
using NSubstitute;
using RuanFa.Shop.Application.Common.Behaviours;

namespace RuanFa.Shop.Application.UnitTests.Common.Behaviors;

public class RequestLoggingPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_SuccessfulRequest_PropagatesResult()
    {
        // Arrange
        var request = new SimpleRequest();
        ErrorOr<Response> expectedResponse = Response.Instance;

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(expectedResponse);

        var behavior = new RequestLoggingPipelineBehavior<SimpleRequest, ErrorOr<Response>>();

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Response.Instance, result.Value);
    }

    [Fact]
    public async Task Handle_ErrorRequest_PropagatesErrors()
    {
        // Arrange
        var request = new SimpleRequest();
        var errors = new List<Error> { Error.Failure("Request failed") };
        ErrorOr<Response> expectedResponse = errors;

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(expectedResponse);

        var behavior = new RequestLoggingPipelineBehavior<SimpleRequest, ErrorOr<Response>>();

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equivalent(errors, result.Errors);
    }
}
