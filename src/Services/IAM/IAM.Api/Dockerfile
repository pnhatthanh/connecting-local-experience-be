FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["src/Services/IAM/IAM.Api/IAM.Api.csproj", "src/Services/IAM/IAM.Api/"]
COPY ["src/Services/IAM/IAM.Application/IAM.Application.csproj", "src/Services/IAM/IAM.Application/"]
COPY ["src/Services/IAM/IAM.Domain/IAM.Domain.csproj", "src/Services/IAM/IAM.Domain/"]
COPY ["src/Services/IAM/IAM.Infrastructure/IAM.Infrastructure.csproj", "src/Services/IAM/IAM.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/BuildingBlocks.Presentation.csproj", "src/BuildingBlocks/BuildingBlocks.Presentation/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.EntityFramework/BuildingBlocks.EntityFramework.csproj", "src/BuildingBlocks/BuildingBlocks.EntityFramework/"]
COPY ["src/BuildingBlocks/BuildingBlocks.RabbitMQ/BuildingBlocks.RabbitMQ.csproj", "src/BuildingBlocks/BuildingBlocks.RabbitMQ/"]

RUN dotnet restore src/Services/IAM/IAM.Api/IAM.Api.csproj

COPY ["src/Services/IAM/IAM.Api/", "src/Services/IAM/IAM.Api/"]
COPY ["src/Services/IAM/IAM.Application/", "src/Services/IAM/IAM.Application/"]
COPY ["src/Services/IAM/IAM.Domain/", "src/Services/IAM/IAM.Domain/"]
COPY ["src/Services/IAM/IAM.Infrastructure/", "src/Services/IAM/IAM.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/", "src/BuildingBlocks/BuildingBlocks.Presentation/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.EntityFramework/", "src/BuildingBlocks/BuildingBlocks.EntityFramework/"]
COPY ["src/BuildingBlocks/BuildingBlocks.RabbitMQ/", "src/BuildingBlocks/BuildingBlocks.RabbitMQ/"]

RUN dotnet publish src/Services/IAM/IAM.Api/IAM.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "IAM.Api.dll"]
