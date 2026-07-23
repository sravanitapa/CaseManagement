# Case Management API

A sample Case Management API built using ASP.NET Core and Clean Architecture principles.

## Overview

This project demonstrates how to build a scalable, maintainable, and testable Web API using modern .NET development practices.

## Features

- Clean Architecture
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
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
│   └── CaseManagement.Domain.Tests
│
└── CaseManagement.sln
```

## Prerequisites

- .NET SDK 9.0 or later
- Visual Studio 2022/2026
- SQL Server

## Getting Started

### Clone the repository

```bash
git clone <repository-url>
```

### Restore packages

```bash
dotnet restore
```

### Build the solution

```bash
dotnet build
```

### Run the application

```bash
dotnet run
```

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger
- xUnit

## Future Enhancements

- Role-based Authorization
- Refresh Tokens
- Docker Support
- CI/CD Pipeline
- Azure Deployment
- Logging with Serilog
- Caching
- Integration Tests

## License

This project is intended for learning and demonstration purposes.
