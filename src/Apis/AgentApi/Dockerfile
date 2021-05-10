#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Apis/AgentApi/AgentApi.csproj", "src/Apis/AgentApi/"]
RUN dotnet restore "src/Apis/AgentApi/AgentApi.csproj"
COPY . .
WORKDIR "/src/src/Apis/AgentApi"
RUN dotnet build "AgentApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgentApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgentApi.dll"]