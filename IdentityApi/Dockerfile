FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 53258
EXPOSE 44320

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY IdentityApi/IdentityApi.csproj IdentityApi/
RUN dotnet restore IdentityApi/IdentityApi.csproj
COPY . .
WORKDIR /src/IdentityApi
RUN dotnet build IdentityApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IdentityApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityApi.dll"]
