FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50291
EXPOSE 44363

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ../LocationsApi/LocationsApi.csproj ../LocationsApi/
RUN dotnet restore ../LocationsApi/LocationsApi.csproj
COPY . .
WORKDIR /src/../LocationsApi
RUN dotnet build LocationsApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish LocationsApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LocationsApi.dll"]
