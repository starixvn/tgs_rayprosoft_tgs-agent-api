#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Worker/Schedulers/Schedulers.csproj", "Worker/Schedulers/"]
COPY ["Services/BackOffice/BackOffice.csproj", "Services/BackOffice/"]
COPY ["Core/DataServices.Tgs/DataServices.Tgs.csproj", "Core/DataServices.Tgs/"]
COPY ["Core/Models/Models.csproj", "Core/Models/"]
COPY ["Core/Infrastructures/Infrastructures.csproj", "Core/Infrastructures/"]
COPY ["Core/DataServices.Base/DataServices.Base.csproj", "Core/DataServices.Base/"]
COPY ["Services/Hangfire/Hangfire.csproj", "Services/Hangfire/"]
RUN dotnet restore "Worker/Schedulers/Schedulers.csproj"
COPY . .
WORKDIR "/src/Worker/Schedulers"
RUN dotnet build "Schedulers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Schedulers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Worker.Schedulers.dll"]