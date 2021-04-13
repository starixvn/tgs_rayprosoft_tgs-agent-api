#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS publish
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /dest
COPY ["nuget.config", "."]
COPY ["src/Apis/LogApi/LogApi.csproj", "src/Apis/LogApi/"]
COPY ["src/Core/DataServices.Log/DataServices.Log.csproj", "src/Core/DataServices.Log/"]
COPY ["src/Core/Infrastructures/Infrastructures.csproj", "src/Core/Infrastructures/"]
COPY ["src/Core/Models/Models.csproj", "src/Core/Models/"]
COPY ["src/Core/DataServices.Base/DataServices.Base.csproj", "src/Core/DataServices.Base/"]
RUN dotnet restore "src/Apis/LogApi/LogApi.csproj"
COPY . .
WORKDIR "/dest/src/Apis/LogApi"
RUN dotnet publish "LogApi.csproj" -c ${BuildConfiguration} -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.LogApi.dll"]