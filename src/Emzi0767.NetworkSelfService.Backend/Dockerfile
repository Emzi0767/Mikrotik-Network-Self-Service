﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 9000
EXPOSE 9001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Emzi0767.NetworkSelfService.Backend/Emzi0767.NetworkSelfService.Backend.csproj", "src/Emzi0767.NetworkSelfService.Backend/"]
RUN dotnet restore "src/Emzi0767.NetworkSelfService.Backend/Emzi0767.NetworkSelfService.Backend.csproj"
COPY . .
WORKDIR "/src/src/Emzi0767.NetworkSelfService.Backend"
RUN dotnet build "Emzi0767.NetworkSelfService.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Emzi0767.NetworkSelfService.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Emzi0767.NetworkSelfService.Backend.dll"]
