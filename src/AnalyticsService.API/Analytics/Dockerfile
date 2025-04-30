FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8006
#EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AnalyticsService.API/Analytics/Analytics.csproj", "src/AnalyticsService.API/Analytics/"]
COPY ["libs/ReservationSystem.Shared/ReservationSystem.Shared.csproj", "libs/ReservationSystem.Shared/"]
COPY ["src/AnalyticsService.API/Analytics.Application/Analytics.Application.csproj", "src/AnalyticsService.API/Analytics.Application/"]
COPY ["src/AnalyticsService.API/Analytics.Domain/Analytics.Domain.csproj", "src/AnalyticsService.API/Analytics.Domain/"]
COPY ["src/AnalyticsService.API/Analytics.Infrastructure/Analytics.Infrastructure.csproj", "src/AnalyticsService.API/Analytics.Infrastructure/"]
RUN dotnet restore "src/AnalyticsService.API/Analytics/Analytics.csproj"
COPY . .
WORKDIR "/src/src/AnalyticsService.API/Analytics"
RUN dotnet build "Analytics.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Analytics.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Analytics.dll"]

# Spuštění migrací před spuštěním aplikace
CMD ["sh", "-c", "dotnet Analytics.dll && dotnet ef database update"]
