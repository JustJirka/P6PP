FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
#EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY src/BookingService.API/BookingService.API.csproj src/BookingService.API/
COPY libs/ReservationSystem.Shared/ReservationSystem.Shared.csproj libs/ReservationSystem.Shared/
RUN dotnet restore "src/BookingService.API/BookingService.API.csproj"
COPY . .
WORKDIR "/src/src/BookingService.API"
RUN dotnet build "BookingService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BookingService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingService.API.dll"]