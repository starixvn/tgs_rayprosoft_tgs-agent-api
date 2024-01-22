#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["AgentApi/AgentApi.csproj", "AgentApi/"]
COPY ["Agent.Repositories/Agent.Repositories.csproj", "Agent.Repositories/"]
COPY ["Agent.Core/Agent.Core.csproj", "Agent.Core/"]
COPY ["Operations/Operations.csproj", "Operations/"]
COPY ["Wallet.Core/Wallet.Core.csproj", "Wallet.Core/"]
COPY ["Wallet.Repositories/Wallet.Repositories.csproj", "Wallet.Repositories/"]
RUN dotnet restore "./AgentApi/./AgentApi.csproj"
COPY . .
WORKDIR "/src/AgentApi"
RUN dotnet build "./AgentApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AgentApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.AgentApi.dll"]