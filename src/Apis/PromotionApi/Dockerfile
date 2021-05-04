#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build AS publish
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Apis/PromotionApi/PromotionApi.csproj", "src/Apis/PromotionApi/"]
COPY ["src/Services/Report/Report.csproj", "src/Services/Report/"]
COPY ["src/Services/PluggableServices/PluggableServices.csproj", "src/Services/PluggableServices/"]
COPY ["src/Services/Operations/Operations.csproj", "src/Services/Operations/"]
COPY ["src/Core/DataServices.Tgs/DataServices.Tgs.csproj", "src/Core/DataServices.Tgs/"]
COPY ["src/Core/Models/Models.csproj", "src/Core/Models/"]
COPY ["src/Core/Infrastructures/Infrastructures.csproj", "src/Core/Infrastructures/"]
COPY ["src/Core/DataServices.Base/DataServices.Base.csproj", "src/Core/DataServices.Base/"]
COPY ["Services/Promotion.Core/PromotionCore.csproj", "Services/Promotion.Core/"]
COPY ["src/Services/Bucket/PlayerBucket.Core.csproj", "src/Services/Bucket/"]
COPY ["src/Clients/Report.Client/Report.Client.csproj", "src/Clients/Report.Client/"]
COPY ["src/Services/Wallet.Core/Wallet.Core.csproj", "src/Services/Wallet.Core/"]
COPY ["src/Core/DataServices.Report/DataServices.Report.csproj", "src/Core/DataServices.Report/"]
COPY ["src/Repositories/BackOffice.Repositories/BackOffice.Repositories.csproj", "src/Repositories/BackOffice.Repositories/"]
COPY ["src/Services/BackOffice/BackOffice.csproj", "src/Services/BackOffice/"]
COPY ["src/Repositories/PaymentRepositories/PaymentRepositories.csproj", "src/Repositories/PaymentRepositories/"]
COPY ["src/Services/Payment.Core/Payment.Core.csproj", "src/Services/Payment.Core/"]
COPY ["src/Repositories/Wallet.Repositories/Wallet.Repositories.csproj", "src/Repositories/Wallet.Repositories/"]
COPY ["src/Clients/Promotion.Client/Promotion.Client.csproj", "src/Clients/Promotion.Client/"]
COPY ["src/Repositories/Promotion.Repositories/Promotion.Repositories.csproj", "src/Repositories/Promotion.Repositories/"]
COPY ["src/Repositories/PlayerBucket.Repositories/PlayerBucket.Repositories.csproj", "src/Repositories/PlayerBucket.Repositories/"]
RUN dotnet restore "src/Apis/PromotionApi/PromotionApi.csproj"
COPY . .
WORKDIR "/src/src/Apis/PromotionApi"
RUN dotnet build "PromotionApi.csproj" -c ${BuildConfiguration} -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PromotionApi.dll"]