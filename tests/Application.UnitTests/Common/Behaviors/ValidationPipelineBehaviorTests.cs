using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using RuanFa.Shop.Application.Common.Behaviours;

namespace RuanFa.Shop.Application.UnitTests.Common.Behaviors;

public class ValidationPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidator_ProceedsToNextHandler()
    {
        // Arrange
        var request = new SimpleRequest();
        ErrorOr<Response> expectedResponse = Response.Instance;

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult(expectedResponse));

        var behavior = new ValidationPipelineBehavior<SimpleRequest, ErrorOr<Response>>();

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Response.Instance);
        await next.Received(1)();
    }

    [Fact]
    public async Task Handle_Validator_ValidationSucceeds_ProceedsToNextHandler()
    {
        // Arrange
        var request = new SimpleRequest();
        ErrorOr<Response> expectedResponse = Response.Instance;

        var validator = Substitute.For<IValidator<SimpleRequest>>();
        validator.ValidateAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult()));

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult(expectedResponse));

        var behavior = new ValidationPipelineBehavior<SimpleRequest, ErrorOr<Response>>(validator);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Response.Instance);
        await validator.Received(1).ValidateAsync(request, Arg.Any<CancellationToken>());
        await next.Received(1)();
    }

    [Fact]
    public async Task Handle_Validator_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var request = new SimpleRequest();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Property", "Invalid value") { ErrorCode = "Property" }
        };

        var validator = Substitute.For<IValidator<SimpleRequest>>();
        validator.ValidateAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult(validationFailures)));

        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Response>>>();
        next().Returns(Task.FromResult<ErrorOr<Response>>(Response.Instance)); // Mocked for safety, not called

        var behavior = new ValidationPipelineBehavior<SimpleRequest, ErrorOr<Response>>(validator);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe("Property");
        result.FirstError.Description.ShouldBe("Invalid value");
        await validator.Received(1).ValidateAsync(request, Arg.Any<CancellationToken>());
        await next.DidNotReceive()();
    }
}
