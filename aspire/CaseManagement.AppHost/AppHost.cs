var builder = DistributedApplication.CreateBuilder(args);

//Run PostgreSQL in a container with a persistent data volume.
var postgres = builder.AddPostgres("postgres").WithDataVolume();

var casesdb = postgres.AddDatabase("casesdb");

//Start the API, hand it the database connection, and wait for the
//database to be ready before the API boots.
builder.AddProject<Projects.CaseManagement_Api>("api").WithReference(casesdb).WaitFor(casesdb);

builder.Build().Run();
