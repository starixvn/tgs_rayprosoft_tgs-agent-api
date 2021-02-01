#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS publish
WORKDIR /src
COPY ["Apis/BackendOffice/BackendOffice.csproj", "Apis/BackendOffice/"]
COPY ["Apis/GlobalApi/GlobalApi.csproj", "Apis/GlobalApi/"]
COPY ["Services/BackOffice/BackOffice.csproj", "Services/BackOffice/"]
COPY ["Services/Report/Report.csproj", "Services/Report/"]
COPY ["Services/Operations/Operations.csproj", "Services/Operations/"]
COPY ["Core/Models/Models.csproj", "Core/Models/"]
COPY ["Core/Infrastructures/Infrastructures.csproj", "Core/Infrastructures/"]
COPY ["Core/DataServices.Base/DataServices.Base.csproj", "Core/DataServices.Base/"]
COPY ["Core/DataServices.Log/DataServices.Log.csproj", "Core/DataServices.Log/"]
COPY ["Core/DataServices.Tgs/DataServices.Tgs.csproj", "Core/DataServices.Tgs/"]
COPY ["Core/DataServices.Report/DataServices.Report.csproj", "Core/DataServices.Report/"]
RUN dotnet restore "Apis/BackendOffice/BackendOffice.csproj"
COPY . .
WORKDIR "/src/Apis/BackendOffice"
RUN dotnet publish "BackendOffice.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.BackendOffice.dll"]