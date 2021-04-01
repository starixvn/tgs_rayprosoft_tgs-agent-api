#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-amd64 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS build
WORKDIR /dest
COPY ["nuget.config", "."]
COPY ["src/Workers/Schedulers/Schedulers.csproj", "src/Workers/Schedulers/"]
COPY ["src/Services/BackOffice/BackOffice.csproj", "src/Services/BackOffice/"]
COPY ["src/Core/DataServices.Tgs/DataServices.Tgs.csproj", "src/Core/DataServices.Tgs/"]
COPY ["src/Core/Models/Models.csproj", "src/Core/Models/"]
COPY ["src/Core/Infrastructures/Infrastructures.csproj", "src/Core/Infrastructures/"]
COPY ["src/Core/DataServices.Base/DataServices.Base.csproj", "src/Core/DataServices.Base/"]
COPY ["src/Services/Report/Report.csproj", "src/Services/Report/"]
COPY ["src/Services/PluggableServices/PluggableServices.csproj", "src/Services/PluggableServices/"]
COPY ["src/Services/Operations/Operations.csproj", "src/Services/Operations/"]
COPY ["src/Clients/Report.Client/Report.Client.csproj", "src/Clients/Report.Client/"]
COPY ["src/Core/DataServices.Report/DataServices.Report.csproj", "src/Core/DataServices.Report/"]
COPY ["src/Shared/Game.ApiClients/Game.ApiClients.csproj", "src/Shared/Game.ApiClients/"]
COPY ["src/Shared/Game.Models/Game.Models.csproj", "src/Shared/Game.Models/"]
RUN dotnet restore "src/Workers/Schedulers/Schedulers.csproj"
COPY . .
WORKDIR "/dest/src/Workers/Schedulers"
RUN dotnet publish "Schedulers.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Workers.Schedulers.dll"]