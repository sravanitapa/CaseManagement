
# ----- Build stage -----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first (layer caching - restore only reruns if these change)
COPY src/CaseManagement.Api/CaseManagement.Api.csproj src/CaseManagement.Api/
COPY src/CaseManagement.Application/CaseManagement.Application.csproj src/CaseManagement.Application/
COPY src/CaseManagement.Domain/CaseManagement.Domain.csproj src/CaseManagement.Domain/
COPY src/CaseManagement.Infrastructure/CaseManagement.Infrastructure.csproj src/CaseManagement.Infrastructure/
COPY Directory.Packages.props Directory.Packages.props ./
COPY Directory.Build.props Directory.Build.props ./
COPY aspire/CaseManagement.ServiceDefaults/CaseManagement.ServiceDefaults.csproj aspire/CaseManagement.ServiceDefaults/

RUN dotnet restore src/CaseManagement.Api/CaseManagement.Api.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/src/CaseManagement.Api
RUN dotnet build CaseManagement.Api.csproj -c Release -o /app/build

# ----- Publish stage -----
FROM build AS publish
RUN dotnet publish CaseManagement.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ----- Final stage -----

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the build output from the build stage
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","CaseManagement.Api.dll"]
