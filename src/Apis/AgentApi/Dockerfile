#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Apis/AgentApi/AgentApi.csproj", "src/Apis/AgentApi/"]
COPY ["src/Repositories/Agent.Repositories/Agent.Repositories.csproj", "src/Repositories/Agent.Repositories/"]
COPY ["src/Services/Agent.Core/Agent.Core.csproj", "src/Services/Agent.Core/"]
COPY ["src/Services/Operations/Operations.csproj", "src/Services/Operations/"]
COPY ["src/Services/Wallet.Core/Wallet.Core.csproj", "src/Services/Wallet.Core/"]
COPY ["src/Repositories/Wallet.Repositories/Wallet.Repositories.csproj", "src/Repositories/Wallet.Repositories/"]
COPY ["src/Services/PluggableServices/PluggableServices.csproj", "src/Services/PluggableServices/"]
RUN dotnet restore "src/Apis/AgentApi/AgentApi.csproj"
COPY . .
WORKDIR "/src/src/Apis/AgentApi"
RUN dotnet build "AgentApi.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "AgentApi.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.AgentApi.dll"]