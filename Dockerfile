# syntax=docker/dockerfile:1
FROM node:22-bookworm-slim AS frontend
WORKDIR /src/CovidPass.Front
COPY CovidPass.Front/package.json CovidPass.Front/package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY CovidPass.Front/ ./
ENV NODE_OPTIONS=--openssl-legacy-provider
RUN npx cross-env NODE_ENV=production webpack --hide-modules

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src
COPY CovidPass.sln ./
COPY CovidPass.Back/CovidPass.Back.csproj CovidPass.Back/
COPY CovidPass.Common/CovidPass.Common.csproj CovidPass.Common/
COPY CovidPass.Core/CovidPass.Core.csproj CovidPass.Core/
COPY CovidPass.Db/CovidPass.Db.csproj CovidPass.Db/
RUN dotnet restore CovidPass.Back/CovidPass.Back.csproj
COPY CovidPass.Back/ CovidPass.Back/
COPY CovidPass.Common/ CovidPass.Common/
COPY CovidPass.Core/ CovidPass.Core/
COPY CovidPass.Db/ CovidPass.Db/
COPY --from=frontend /src/CovidPass.Back/wwwroot CovidPass.Back/wwwroot
RUN dotnet publish CovidPass.Back/CovidPass.Back.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=backend /app ./
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    RewriteUrlSection=^employee/ \
    Auth__Mode=EntraId \
    Logging__LogLevel__Default=Information
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "CovidPass.Back.dll"]
