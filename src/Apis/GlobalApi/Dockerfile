#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS publish
WORKDIR /dest
COPY ["src/Apis/GlobalApi/GlobalApi.csproj", "src/Apis/GlobalApi/"]
COPY ["src/Services/Operations/Operations.csproj", "src/Services/Operations/"]
COPY ["src/Core/DataServices.Tgs/DataServices.Tgs.csproj", "src/Core/DataServices.Tgs/"]
COPY ["src/Core/Models/Models.csproj", "src/Core/Models/"]
COPY ["src/Core/Infrastructures/Infrastructures.csproj", "src/Core/Infrastructures/"]
COPY ["src/Core/DataServices.Base/DataServices.Base.csproj", "src/Core/DataServices.Base/"]
RUN dotnet restore "src/Apis/GlobalApi/GlobalApi.csproj"
COPY . .
WORKDIR "/dest/src/Apis/GlobalApi"
RUN dotnet publish "GlobalApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.GlobalApi.dll"]