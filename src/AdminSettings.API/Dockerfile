FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9090
EXPOSE 8081

# ✅ Přidej mysql-client do base imagu
RUN apt-get update && apt-get install -y default-mysql-client

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/AdminSettings.API/AdminSettings.API.csproj", "src/AdminSettings.API/"]
COPY ["libs/ReservationSystem.Shared/ReservationSystem.Shared.csproj", "libs/ReservationSystem.Shared/"]
RUN dotnet restore "src/AdminSettings.API/AdminSettings.API.csproj"
COPY . .
WORKDIR "/src/src/AdminSettings.API"
RUN dotnet build "AdminSettings.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AdminSettings.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminSettings.API.dll"]
