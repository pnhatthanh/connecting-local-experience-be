FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY ["src/Services/Experience/Experience.Api/Experience.Api.csproj", "src/Services/Experience/Experience.Api/"]
COPY ["src/Services/Experience/Experience.Application/Experience.Application.csproj", "src/Services/Experience/Experience.Application/"]
COPY ["src/Services/Experience/Experience.Domain/Experience.Domain.csproj", "src/Services/Experience/Experience.Domain/"]
COPY ["src/Services/Experience/Experience.Infrastructure/Experience.Infrastructure.csproj", "src/Services/Experience/Experience.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.EntityFramework/BuildingBlocks.EntityFramework.csproj", "src/BuildingBlocks/BuildingBlocks.EntityFramework/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/BuildingBlocks.Presentation.csproj", "src/BuildingBlocks/BuildingBlocks.Presentation/"]

RUN dotnet restore src/Services/Experience/Experience.Api/Experience.Api.csproj

# Copy source code
COPY ["src/Services/Experience/", "src/Services/Experience/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.EntityFramework/", "src/BuildingBlocks/BuildingBlocks.EntityFramework/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/", "src/BuildingBlocks/BuildingBlocks.Presentation/"]

RUN dotnet publish src/Services/Experience/Experience.Api/Experience.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Experience.Api.dll"]
