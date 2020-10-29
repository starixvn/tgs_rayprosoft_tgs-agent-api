#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Apis/LogApi/LogApi.csproj", "Apis/LogApi/"]
COPY ["Core/DataServices.Log/DataServices.Log.csproj", "Core/DataServices.Log/"]
COPY ["Core/Infrastructures/Infrastructures.csproj", "Core/Infrastructures/"]
COPY ["Core/Models/Models.csproj", "Core/Models/"]
COPY ["Core/DataServices.Base/DataServices.Base.csproj", "Core/DataServices.Base/"]
RUN dotnet restore "Apis/LogApi/LogApi.csproj"
COPY . .
WORKDIR "/src/Apis/LogApi"
RUN dotnet build "LogApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LogApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.LogApi.dll"]