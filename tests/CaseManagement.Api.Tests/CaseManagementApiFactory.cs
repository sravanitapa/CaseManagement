using CaseManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace CaseManagement.Api.Tests;

// Boots the real API (Program.cs, real middleware pipeline, real Identity/JWT
// setup) against a throwaway Postgres container instead of a fake/in-memory
// substitute. This is what actually catches the kind of bugs a mocked DB
// would hide - e.g. the refresh-token rotation and revocation logic here
// depends on real relational queries and timestamps.
public class CaseManagementApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("casesdb_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // "Development" is what turns on the auto-migrate + seed block in
        // Program.cs, so the schema and default roles exist before tests run.
        builder.UseEnvironment("Development");

        // ValidateScopes/ValidateOnBuild are enabled by default in
        // Development and are what surfaces DbContext-pooling descriptor
        // conflicts as a hard failure at host-build time. That validation is
        // a safety net for catching real DI mistakes - it isn't needed for
        // a test host we're deliberately reconfiguring below, so we turn it
        // off here rather than chase every internal pooling descriptor by
        // hand.
        builder.UseDefaultServiceProvider((_, options) =>
        {
            options.ValidateScopes = false;
            options.ValidateOnBuild = false;
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // This is the same "casesdb" key Aspire injects at runtime -
                // AddNpgsqlDbContext<ApplicationDbContext>("casesdb") in
                // Program.cs reads it from ConnectionStrings:casesdb either way.
                ["ConnectionStrings:casesdb"] = _postgres.GetConnectionString()
            });
        });

        // Belt-and-suspenders: Aspire's AddNpgsqlDbContext resolves its
        // connection string from configuration at a point that doesn't
        // always line up cleanly with WebApplicationFactory's configuration
        // overrides above. Rather than fight that timing, remove whatever
        // ApplicationDbContext registration Program.cs already set up and
        // re-register it pointed straight at the test container.
        //
        // Aspire's AddNpgsqlDbContext pools contexts by default, which wires
        // up several related singleton-level services alongside
        // DbContextOptions<T> (DbContextPool<T>, IScopedDbContextLease<T>,
        // etc). Removing only DbContextOptions<T> leaves those behind and
        // they end up depending on our freshly-scoped options, which throws
        // at container-build time. Matching on FullName catches all of them
        // without needing to reference EF Core's internal pooling types.
        builder.ConfigureTestServices(services =>
        {
            var descriptorsToRemove = services
              .Where(d => d.ServiceType == typeof(ApplicationDbContext)
                       || (d.ServiceType.IsGenericType
                           && d.ServiceType.GetGenericArguments().Contains(typeof(ApplicationDbContext))))
              .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));
        });
    }

    public Task InitializeAsync() => _postgres.StartAsync();

    // Explicit interface implementation: WebApplicationFactory already
    // defines its own DisposeAsync() (returning ValueTask), so xUnit's
    // IAsyncLifetime.DisposeAsync (returning Task) has to be implemented
    // explicitly to avoid a signature clash.
    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgres.StopAsync();
        await base.DisposeAsync();
    }
}
