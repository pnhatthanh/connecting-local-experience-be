FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["src/Services/User/User.Api/User.Api.csproj", "src/Services/User/User.Api/"]
COPY ["src/Services/User/User.Application/User.Application.csproj", "src/Services/User/User.Application/"]
COPY ["src/Services/User/User.Domain/User.Domain.csproj", "src/Services/User/User.Domain/"]
COPY ["src/Services/User/User.Infrastructure/User.Infrastructure.csproj", "src/Services/User/User.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/BuildingBlocks.Presentation.csproj", "src/BuildingBlocks/BuildingBlocks.Presentation/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.EntityFramework/BuildingBlocks.EntityFramework.csproj", "src/BuildingBlocks/BuildingBlocks.EntityFramework/"]
COPY ["src/BuildingBlocks/BuildingBlocks.RabbitMQ/BuildingBlocks.RabbitMQ.csproj", "src/BuildingBlocks/BuildingBlocks.RabbitMQ/"]

RUN dotnet restore src/Services/User/User.Api/User.Api.csproj

COPY ["src/Services/User/User.Api/", "src/Services/User/User.Api/"]
COPY ["src/Services/User/User.Application/", "src/Services/User/User.Application/"]
COPY ["src/Services/User/User.Domain/", "src/Services/User/User.Domain/"]
COPY ["src/Services/User/User.Infrastructure/", "src/Services/User/User.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/", "src/BuildingBlocks/BuildingBlocks.Presentation/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/", "src/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.EntityFramework/", "src/BuildingBlocks/BuildingBlocks.EntityFramework/"]
COPY ["src/BuildingBlocks/BuildingBlocks.RabbitMQ/", "src/BuildingBlocks/BuildingBlocks.RabbitMQ/"]

RUN dotnet publish src/Services/User/User.Api/User.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "User.Api.dll"]
