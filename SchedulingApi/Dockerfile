FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50327
EXPOSE 44322

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY SchedulingApi/SchedulingApi.csproj SchedulingApi/
RUN dotnet restore SchedulingApi/SchedulingApi.csproj
COPY . .
WORKDIR /src/SchedulingApi
RUN dotnet build SchedulingApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SchedulingApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SchedulingApi.dll"]
