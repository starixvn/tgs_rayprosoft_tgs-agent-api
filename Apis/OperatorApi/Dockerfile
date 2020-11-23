#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Apis/OperatorApi/OperatorApi.csproj", "Apis/OperatorApi/"]
COPY ["Services/Clients/Clients.csproj", "Services/Clients/"]
COPY ["Services/Operations/Operations.csproj", "Services/Operations/"]
COPY ["Core/DataServices/DataServices.csproj", "Core/DataServices/"]
COPY ["Core/Models/Models.csproj", "Core/Models/"]
COPY ["Core/Infrastructures/Infrastructures.csproj", "Core/Infrastructures/"]
COPY ["Core/DataServices.Base/DataServices.Base.csproj", "Core/DataServices.Base/"]
COPY ["Core/DataServices.Bet/DataServices.Bet.csproj", "Core/DataServices.Bet/"]
RUN dotnet restore "Apis/OperatorApi/OperatorApi.csproj"
COPY . .
WORKDIR "/src/Apis/OperatorApi"
RUN dotnet build "OperatorApi.csproj" -c Release -o /app/build

RUN dotnet dev-certs https --trust

FROM build AS publish
RUN dotnet publish "OperatorApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.OperatorApi.dll"]