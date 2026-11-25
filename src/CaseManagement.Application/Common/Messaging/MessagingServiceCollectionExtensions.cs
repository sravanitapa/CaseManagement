using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CaseManagement.Application.Common.Messaging;

public static class MessagingServiceCollectionExtensions
{
    /// <summary>Registers <see cref="ISender"/> and every <see cref="IRequestHandler{TRequest,TResponse}"/> in the assembly.</summary>
    public static IServiceCollection AddMessaging(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<ISender, Sender>();

        var handlerInterface = typeof(IRequestHandler<,>);
        var handlerTypes = assembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false });

        foreach (var type in handlerTypes)
        {
            foreach (var implemented in type.GetInterfaces())
            {
                if (implemented.IsGenericType && implemented.GetGenericTypeDefinition() == handlerInterface)
                {
                    services.AddScoped(implemented, type);
                }
            }
        }

        return services;
    }
}
