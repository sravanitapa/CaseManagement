using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using CaseManagement.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CaseManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        //Hand the Application layer its IApplicationDbContext,backed by the
        //real ApplicationdbContext.Aspire registers the DbContext itself in
        //the real API project, so here we only map the interface to the concrete type.
        //Whenever someone asks for IApplicationDbContext, give them the existing ApplicationDbContext object.

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        return services;
    }
}
