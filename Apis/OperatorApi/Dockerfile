#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS publish
WORKDIR /src
COPY ["Apis/OperatorApi/OperatorApi.csproj", "Apis/OperatorApi/"]
COPY ["Core/DataServices.Tgs/DataServices.Tgs.csproj", "Core/DataServices.Tgs/"]
COPY ["Core/Models/Models.csproj", "Core/Models/"]
COPY ["Core/Infrastructures/Infrastructures.csproj", "Core/Infrastructures/"]
COPY ["Core/DataServices.Base/DataServices.Base.csproj", "Core/DataServices.Base/"]
COPY ["Services/Operations/Operations.csproj", "Services/Operations/"]
COPY ["Services/PluggableServices/PluggableServices.csproj", "Services/PluggableServices/"]
RUN dotnet restore "Apis/OperatorApi/OperatorApi.csproj"
COPY . .
WORKDIR "/src/Apis/OperatorApi"
RUN dotnet publish "OperatorApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.OperatorApi.dll"]