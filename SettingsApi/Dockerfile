FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50480
EXPOSE 44364

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY SettingsApi/SettingsApi.csproj SettingsApi/
RUN dotnet restore SettingsApi/SettingsApi.csproj
COPY . .
WORKDIR /src/SettingsApi
RUN dotnet build SettingsApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SettingsApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SettingsApi.dll"]
