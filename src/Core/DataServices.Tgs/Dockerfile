#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /app
WORKDIR /src
COPY . .
WORKDIR "/src/src/Core/DataServices.Tgs"
RUN dotnet tool restore
RUN dotnet build "DataServices.Tgs.csproj" -c Release

FROM build AS publish
RUN dotnet ef migrations bundle -o /app/efbundle
RUN cp tgs.appsettings.json /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["./efbundle"]
