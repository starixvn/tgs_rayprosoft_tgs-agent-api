#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Workers/Schedulers/Schedulers.csproj", "src/Workers/Schedulers/"]
COPY ["src/Services/BackOffice/BackOffice.csproj", "src/Services/BackOffice/"]
COPY ["src/Core/DataServices.Tgs/DataServices.Tgs.csproj", "src/Core/DataServices.Tgs/"]
COPY ["src/Core/Models/Models.csproj", "src/Core/Models/"]
COPY ["src/Core/Infrastructures/Infrastructures.csproj", "src/Core/Infrastructures/"]
COPY ["src/Core/DataServices.Base/DataServices.Base.csproj", "src/Core/DataServices.Base/"]
COPY ["src/Services/Bucket/PlayerBucket.Core.csproj", "src/Services/Bucket/"]
COPY ["src/Repositories/PaymentRepositories/PaymentRepositories.csproj", "src/Repositories/PaymentRepositories/"]
COPY ["src/Services/Payment.Core/Payment.Core.csproj", "src/Services/Payment.Core/"]
COPY ["src/Services/Wallet.Core/Wallet.Core.csproj", "src/Services/Wallet.Core/"]
COPY ["src/Services/Promotion.Core/Promotion.Core.csproj", "src/Services/Promotion.Core/"]
COPY ["src/Services/TgsTransaction.Core/TgsTransaction.Core.csproj", "src/Services/TgsTransaction.Core/"]
COPY ["src/Repositories/TgsTransaction.Repositories/TgsTransaction.Repositories.csproj", "src/Repositories/TgsTransaction.Repositories/"]
COPY ["src/Services/Operations/Operations.csproj", "src/Services/Operations/"]
COPY ["src/Services/Report/Report.csproj", "src/Services/Report/"]
COPY ["src/Services/PluggableServices/PluggableServices.csproj", "src/Services/PluggableServices/"]
COPY ["src/Clients/Report.Client/Report.Client.csproj", "src/Clients/Report.Client/"]
COPY ["src/Core/DataServices.Report/DataServices.Report.csproj", "src/Core/DataServices.Report/"]
COPY ["src/Shared/Game.ApiClients/Game.ApiClients.csproj", "src/Shared/Game.ApiClients/"]
COPY ["src/Shared/Game.Models/Game.Models.csproj", "src/Shared/Game.Models/"]
COPY ["src/Clients/Promotion.Client/Promotion.Client.csproj", "src/Clients/Promotion.Client/"]
COPY ["src/Repositories/GameProviders.Repositories/GameProviders.Repositories.csproj", "src/Repositories/GameProviders.Repositories/"]
RUN dotnet restore "src/Workers/Schedulers/Schedulers.csproj"
COPY . .
WORKDIR "/src/src/Workers/Schedulers"
RUN dotnet build "Schedulers.csproj" -c ${BuildConfiguration} -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Schedulers.csproj" -c ${BuildConfiguration} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Workers.Schedulers.dll"]