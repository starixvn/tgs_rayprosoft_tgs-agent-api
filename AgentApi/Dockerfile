#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["AgentApi/AgentApi.csproj", "AgentApi/"]
COPY ["Agent.Repositories/Agent.Repositories.csproj", "Agent.Repositories/"]
COPY ["Agent.Core/Agent.Core.csproj", "Agent.Core/"]
COPY ["Operations/Operations.csproj", "Operations/"]
COPY ["Wallet.Core/Wallet.Core.csproj", "Wallet.Core/"]
COPY ["Wallet.Repositories/Wallet.Repositories.csproj", "Wallet.Repositories/"]
RUN dotnet restore "AgentApi/AgentApi.csproj"
COPY . .
WORKDIR "/src/AgentApi"
RUN dotnet build "AgentApi.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "AgentApi.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.AgentApi.dll"]