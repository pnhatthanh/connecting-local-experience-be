FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY ["src/Services/Notification/Notification.csproj", "src/Services/Notification/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.RabbitMQ/BuildingBlocks.RabbitMQ.csproj", "src/BuildingBlocks/BuildingBlocks.RabbitMQ/"]

RUN dotnet restore src/Services/Notification/Notification.csproj

# Copy source code
COPY ["src/Services/Notification/", "src/Services/Notification/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.RabbitMQ/", "src/BuildingBlocks/BuildingBlocks.RabbitMQ/"]

RUN dotnet publish src/Services/Notification/Notification.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Notification.dll"]
