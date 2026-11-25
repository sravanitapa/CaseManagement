namespace CaseManagement.Application.Common.Messaging;

/// <summary>Marker for a command or query that produces a <typeparamref name="TResponse"/>.</summary>
public interface IRequest<out TResponse>
{
}

/// <summary>Handles exactly one request type. One handler per request.</summary>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>The "next" step in the pipeline: either the next behavior or the handler itself.</summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Cross-cutting step that wraps every handler (validation, logging, ...).
/// Behaviors run in the order they are registered; the first one registered is the outermost.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

/// <summary>Sends a request through the pipeline to its single handler.</summary>
public interface ISender
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
