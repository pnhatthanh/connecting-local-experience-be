FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["src/APIGateway/APIGateway.csproj", "src/APIGateway/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/BuildingBlocks.Presentation.csproj", "src/BuildingBlocks/BuildingBlocks.Presentation/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "src/BuildingBlocks/BuildingBlocks.Domain/"]

RUN dotnet restore src/APIGateway/APIGateway.csproj

COPY ["src/APIGateway/", "src/APIGateway/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/", "src/BuildingBlocks/BuildingBlocks.Presentation/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/", "src/BuildingBlocks/BuildingBlocks.Domain/"]

RUN dotnet publish src/APIGateway/APIGateway.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build ["/app/publish", "."]

EXPOSE 8080

ENTRYPOINT ["dotnet", "APIGateway.dll"]