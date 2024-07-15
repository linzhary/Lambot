#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
USER root

RUN apt-get update && apt-get install -y sqlite3 libsqlite3-dev

USER app
WORKDIR /app

EXPOSE 9527

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Lambot.Template/Lambot.Template.csproj", "src/Lambot.Template/"]
COPY ["src/adapters/Lambot.Adapters.OneBot/Lambot.Adapters.OneBot.csproj", "src/adapters/Lambot.Adapters.OneBot/"]
COPY ["src/Lambot.Core/Lambot.Core.csproj", "src/Lambot.Core/"]
RUN dotnet restore "./src/Lambot.Template/Lambot.Template.csproj"
COPY . .
WORKDIR "/src/src/Lambot.Template"
RUN dotnet build "./Lambot.Template.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lambot.Template.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lambot.Template.dll"]