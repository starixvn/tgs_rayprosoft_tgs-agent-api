#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Apis/ReportApi/ReportApi.csproj", "src/Apis/ReportApi/"]
COPY ["src/Services/Report/Report.csproj", "src/Services/Report/"]
COPY ["src/Services/PluggableServices/PluggableServices.csproj", "src/Services/PluggableServices/"]
COPY ["src/Services/Operations/Operations.csproj", "src/Services/Operations/"]
COPY ["src/Core/DataServices.Tgs/DataServices.Tgs.csproj", "src/Core/DataServices.Tgs/"]
COPY ["src/Core/Models/Models.csproj", "src/Core/Models/"]
COPY ["src/Core/Infrastructures/Infrastructures.csproj", "src/Core/Infrastructures/"]
COPY ["src/Core/DataServices.Base/DataServices.Base.csproj", "src/Core/DataServices.Base/"]
COPY ["src/Repositories/Wallet.Repositories/Wallet.Repositories.csproj", "src/Repositories/Wallet.Repositories/"]
COPY ["src/Services/Wallet.Core/Wallet.Core.csproj", "src/Services/Wallet.Core/"]
COPY ["src/Repositories/PlayerBucket.Repositories/PlayerBucket.Repositories.csproj", "src/Repositories/PlayerBucket.Repositories/"]
COPY ["src/Services/Bucket/PlayerBucket.Core.csproj", "src/Services/Bucket/"]
COPY ["src/Repositories/BackOffice.Repositories/BackOffice.Repositories.csproj", "src/Repositories/BackOffice.Repositories/"]
COPY ["src/Services/BackOffice/BackOffice.csproj", "src/Services/BackOffice/"]
COPY ["src/Clients/Promotion.Client/Promotion.Client.csproj", "src/Clients/Promotion.Client/"]
COPY ["src/Services/TgsTransaction.Core/TgsTransaction.Core.csproj", "src/Services/TgsTransaction.Core/"]
COPY ["src/Repositories/TgsTransaction.Repositories/TgsTransaction.Repositories.csproj", "src/Repositories/TgsTransaction.Repositories/"]
COPY ["src/Repositories/Promotion.Repositories/Promotion.Repositories.csproj", "src/Repositories/Promotion.Repositories/"]
COPY ["src/Services/Promotion.Core/Promotion.Core.csproj", "src/Services/Promotion.Core/"]
RUN dotnet restore "src/Apis/ReportApi/ReportApi.csproj"
COPY . .
WORKDIR "/src/src/Apis/ReportApi"
RUN dotnet build "ReportApi.csproj" -c ${BuildConfiguration} -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "ReportApi.csproj" -c ${BuildConfiguration} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportApi.dll"]