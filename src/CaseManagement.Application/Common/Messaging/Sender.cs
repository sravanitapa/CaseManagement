using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace CaseManagement.Application.Common.Messaging;

/// <summary>
/// Small in-house request dispatcher (about 60 lines, no third-party dependency).
/// Resolves the handler and any pipeline behaviors from DI and runs them in order.
/// </summary>
internal sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    // One cached wrapper per request type, so reflection runs only once per type.
    private static readonly ConcurrentDictionary<Type, object> Wrappers = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wrapper = (RequestWrapper<TResponse>)Wrappers.GetOrAdd(
            request.GetType(),
            static requestType => Activator.CreateInstance(
                typeof(RequestWrapper<,>).MakeGenericType(requestType, typeof(TResponse)))!);

        return wrapper.Handle(request, serviceProvider, cancellationToken);
    }
}

internal abstract class RequestWrapper<TResponse>
{
    public abstract Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider services, CancellationToken cancellationToken);
}

internal sealed class RequestWrapper<TRequest, TResponse> : RequestWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider services, CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;
        var handler = services.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = services.GetServices<IPipelineBehavior<TRequest, TResponse>>().ToArray();

        // Build the chain from the inside out so behaviors[0] ends up outermost.
        RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle(typedRequest, cancellationToken);
        for (var i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = () => behavior.Handle(typedRequest, next, cancellationToken);
        }

        return pipeline();
    }
}
