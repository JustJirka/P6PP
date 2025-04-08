FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AnalyticsService.API/Analytics/Analytics.csproj", "src/AnalyticsService.API/Analytics/"]
COPY ["libs/ReservationSystem.Shared/ReservationSystem.Shared.csproj", "libs/ReservationSystem.Shared/"]
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
