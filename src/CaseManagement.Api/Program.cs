using System.Text.Json.Serialization;
using CaseManagement.Api.Endpoints;
using CaseManagement.Api.Infrastructure;
using CaseManagement.Application;
using CaseManagement.Infrastructure;
using CaseManagement.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Aspire: OpenTelemetry, health checks, service discovery, resilient HTTP.
builder.AddServiceDefaults();

//Aspire:Aspire reads the "casesdb" connection string it injected and registers
//ApplicationDbContext with the Npgsql provider - in a single call.
builder.AddNpgsqlDbContext<ApplicationDbContext>("casesdb");

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddOpenApi();

//Accept and return enums as their string names ("Action") instead of numbers.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

//Turn domain rule violations into 400 ProblemDetails responses.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

var app = builder.Build();

//DEMO ONLY:create and migrate the database, then seed sample data, so the app
//runs with a single command. Never auto-migrate or auto-speed in production.

if(app.Environment.IsDevelopment())
{
    await app.Services.InitializeDatabaseAsync();
}

app.UseExceptionHandler();

app.MapDefaultEndpoints();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapCaseEndpoints();

app.Run();

//app.MapGet("/", () => "Hello World!");

//app.Run();
