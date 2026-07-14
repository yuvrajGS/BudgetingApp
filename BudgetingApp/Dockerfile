# ASP.NET Backend Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /src

# Copy project files
COPY BudgetingApp.csproj .
COPY . .

# Restore and build
RUN dotnet restore "BudgetingApp.csproj"
RUN dotnet build "BudgetingApp.csproj" -c Release -o /app/build

# Publish
FROM builder AS publish
RUN dotnet publish "BudgetingApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

EXPOSE 7103

ENTRYPOINT ["dotnet", "BudgetingApp.dll"]
