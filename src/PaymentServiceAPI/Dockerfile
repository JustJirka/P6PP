FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5185

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PaymentService.API/PaymentService.API.csproj", "src/PaymentService.API/"]
COPY ["libs/ReservationSystem.Shared/ReservationSystem.Shared.csproj", "libs/ReservationSystem.Shared/"]
RUN dotnet restore "src/PaymentService.API/PaymentService.API.csproj"
COPY . .
WORKDIR "/src/src/PaymentService.API"
RUN dotnet build "PaymentService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.API.dll"]
