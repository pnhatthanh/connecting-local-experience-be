FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Services/Booking/Booking.Api/Booking.Api.csproj", "Services/Booking/Booking.Api/"]
COPY ["src/Services/Booking/Booking.Application/Booking.Application.csproj", "Services/Booking/Booking.Application/"]
COPY ["src/Services/Booking/Booking.Domain/Booking.Domain.csproj", "Services/Booking/Booking.Domain/"]
COPY ["src/Services/Booking/Booking.Infrastructure/Booking.Infrastructure.csproj", "Services/Booking/Booking.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.EntityFramework/BuildingBlocks.EntityFramework.csproj", "BuildingBlocks/BuildingBlocks.EntityFramework/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Presentation/BuildingBlocks.Presentation.csproj", "BuildingBlocks/BuildingBlocks.Presentation/"]
COPY ["src/BuildingBlocks/BuildingBlocks.RabbitMQ/BuildingBlocks.RabbitMQ.csproj", "BuildingBlocks/BuildingBlocks.RabbitMQ/"]

RUN dotnet restore "Services/Booking/Booking.Api/Booking.Api.csproj"

COPY src/ .

WORKDIR "/src/Services/Booking/Booking.Api"
RUN dotnet build "Booking.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Booking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Api.dll"]
