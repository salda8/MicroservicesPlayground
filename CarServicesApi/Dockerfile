FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50307
EXPOSE 44399

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CarServicesApi/CarServicesApi.csproj CarServicesApi/
RUN dotnet restore CarServicesApi/CarServicesApi.csproj
COPY . .
WORKDIR /src/CarServicesApi
RUN dotnet build CarServicesApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CarServicesApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CarServicesApi.dll"]
