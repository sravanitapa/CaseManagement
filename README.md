# Case Management API

A sample Case Management API built using ASP.NET Core and Clean Architecture principles.

## Overview

This project demonstrates how to build a scalable, maintainable, and testable Web API using modern .NET development practices.

## Features

- Clean Architecture
- ASP.NET Core Web API
- Entity Framework Core with PostgreSQL
- JWT Authentication with refresh token rotation
- Role-based Authorization
- Docker & Docker Compose
- Dependency Injection
- Global Exception Handling
- RESTful APIs
- Swagger / OpenAPI
- Unit Testing

## Project Structure

```
CaseManagement
│
├── src
│   ├── CaseManagement.Api
│   ├── CaseManagement.Application
│   ├── CaseManagement.Domain
│   ├── CaseManagement.Infrastructure
│   └── ...
│
├── tests
│   └── CaseManagement.Api.Tests
│   └── CaseManagement.Domain.Tests
|        
└── CaseManagement.sln
```

## Prerequisites

- .NET SDK 10.0 or later
- Docker Desktop (for containerized run)
- Visual Studio 2022/2026

## Getting Started

### Run with Docker (recommended)
\`\`\`bash
docker compose up --build
\`\`\`
API available at http://localhost:8080

### Run locally
\`\`\`bash
dotnet restore
dotnet build
dotnet run --project src/CaseManagement.Api
\`\`\`

## Technologies Used

- ASP.NET Core, Entity Framework Core, PostgreSQL, .NET Aspire
- JWT Authentication + Refresh Tokens
- Docker, Swagger, xUnit

## Future Enhancements

- CQRS with MediatR
- Azure Deployment
- Logging with Serilog
- Caching
- Integration Tests

## Architecture
![Clean Architecture](docs/architecture.png)

## License

This project is intended for learning and demonstration purposes.
