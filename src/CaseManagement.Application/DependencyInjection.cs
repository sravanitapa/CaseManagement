using CaseManagement.Application.Common.Behaviors;
using CaseManagement.Application.Common.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CaseManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Register every IValidator in this assembly (internal ones included).
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // In-house request dispatcher: registers ISender and every request handler.
        services.AddMessaging(assembly);

        // Pipeline behaviors run in registration order (first registered = outermost).
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
