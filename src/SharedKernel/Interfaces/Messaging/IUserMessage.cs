namespace RuanFa.Shop.SharedKernel.Interfaces.Messaging;

/// <summary>
/// Marker interface for messages (commands or queries) associated with a user, providing a user ID.
/// </summary>
public interface IUserMessage
{
    /// <summary>
    /// The ID of the user associated with the message.
    /// </summary>
    Guid UserId { get; set; }
}

/// <summary>
/// Defines a command associated with a user that performs an action and returns a success indicator without a specific response.
/// </summary>
public interface IUserCommand : ICommand, IUserMessage;

/// <summary>
/// Defines a command associated with a user that performs an action and returns a specific response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface IUserCommand<TResponse> : ICommand<TResponse>, IUserMessage;

/// <summary>
/// Defines a query associated with a user that retrieves data and returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IUserQuery<TResponse> : IQuery<TResponse>, IUserMessage;
