using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using FluentValidation;
using CaseManagement.Application.Cases;
using Microsoft.Extensions.DependencyInjection;
using CaseManagement.Application.Common.Behaviors;

namespace CaseManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddScoped<ICaseService, CaseService>();

        //Register every IValidator in this assembly (CreateCaseRequestValidator,etc.).
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}
