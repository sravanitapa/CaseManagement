using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using CaseManagement.Application.Cases;
using Microsoft.Extensions.DependencyInjection;

namespace CaseManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICaseService, CaseService>();

        //Register every IValidator in this assembly (CreateCaseRequestValidator,etc.).
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
