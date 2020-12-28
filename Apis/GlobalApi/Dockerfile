#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS publish
WORKDIR /src
COPY ["Apis/GlobalApi/GlobalApi.csproj", "Apis/GlobalApi/"]
COPY ["Services/Operations/Operations.csproj", "Services/Operations/"]
COPY ["Core/DataServices.Tgs/DataServices.Tgs.csproj", "Core/DataServices.Tgs/"]
COPY ["Core/Models/Models.csproj", "Core/Models/"]
COPY ["Core/Infrastructures/Infrastructures.csproj", "Core/Infrastructures/"]
COPY ["Core/DataServices.Base/DataServices.Base.csproj", "Core/DataServices.Base/"]
COPY ["Core/DataServices.Bet/DataServices.Bet.csproj", "Core/DataServices.Bet/"]
RUN dotnet restore "Apis/GlobalApi/GlobalApi.csproj"
COPY . .
WORKDIR "/src/Apis/GlobalApi"
RUN dotnet publish "GlobalApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.GlobalApi.dll"]