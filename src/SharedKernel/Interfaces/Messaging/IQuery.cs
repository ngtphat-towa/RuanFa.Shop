using ErrorOr;
using MediatR;

namespace RuanFa.Shop.SharedKernel.Interfaces.Messaging;

/// <summary>
/// Defines a query that retrieves data and returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>;

/// <summary>
/// Defines a handler for a query that returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the query to handle, which must implement <see cref="IQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ErrorOr<TResponse>>
    where TQuery : IQuery<TResponse>;
