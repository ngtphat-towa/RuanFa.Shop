using ErrorOr;
using MediatR;

namespace RuanFa.Shop.SharedKernel.Interfaces.Messaging;

/// <summary>
/// Defines a command that performs an action and returns a success indicator without a specific response.
/// </summary>
public interface ICommand : IRequest<ErrorOr<Success>>, IBaseCommand;

/// <summary>
/// Defines a command that performs an action and returns a specific response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>, IBaseCommand;

/// <summary>
/// Marker interface for grouping all command types.
/// </summary>
public interface IBaseCommand;

/// <summary>
/// Defines a handler for a command that returns a success indicator without a specific response.
/// </summary>
/// <typeparam name="TCommand">The type of the command to handle, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, ErrorOr<Success>>
    where TCommand : ICommand;

/// <summary>
/// Defines a handler for a command that returns a specific response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command to handle, which must implement <see cref="ICommand{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ErrorOr<TResponse>>
    where TCommand : ICommand<TResponse>;
