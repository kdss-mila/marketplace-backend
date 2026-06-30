# =========================================================================
# Dockerfile - Marketplace.Api (.NET 9 / ASP.NET Core)
#
# Build multi-stage:
#   1) "base"    -> imagem runtime enxuta (aspnet:9.0)
#   2) "build"   -> SDK 9.0 para restore + build
#   3) "publish" -> dotnet publish em modo Release
#   4) "final"   -> copia o publish para a imagem runtime, ajusta usuário
# =========================================================================

# ---- 1) Runtime base ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# ---- 2) Build / restore -------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia primeiro APENAS a solution + csproj de cada projeto para aproveitar
# o cache de camadas do Docker no "dotnet restore" (só invalida quando algum
# csproj muda, não a cada alteração de código).
COPY ["Marketplace.sln", "./"]
COPY ["01-Api/Marketplace.Api/Marketplace.Api.csproj",                       "01-Api/Marketplace.Api/"]
COPY ["02-Domain/Marketplace.Domain/Marketplace.Domain.csproj",              "02-Domain/Marketplace.Domain/"]
COPY ["02-Domain/Marketplace.Application/Marketplace.Application.csproj",    "02-Domain/Marketplace.Application/"]
COPY ["03-Infra/Marketplace.Repository/Marketplace.Repository.csproj",       "03-Infra/Marketplace.Repository/"]
COPY ["03-Infra/Marketplace.Infrastructure/Marketplace.Infrastructure.csproj","03-Infra/Marketplace.Infrastructure/"]
COPY ["03-Infra/Marketplace.Setup/Marketplace.Setup.csproj",                 "03-Infra/Marketplace.Setup/"]

RUN dotnet restore "Marketplace.sln"

# Agora copia o restante do código e compila o projeto API
COPY . .
WORKDIR "/src/01-Api/Marketplace.Api"
RUN dotnet build "Marketplace.Api.csproj" -c Release -o /app/build --no-restore

# ---- 3) Publish ---------------------------------------------------------
FROM build AS publish
RUN dotnet publish "Marketplace.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---- 4) Imagem final ----------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Garante a pasta usada pelo LocalFileStorageService (wwwroot/uploads).
# O docker-compose monta um volume nomeado neste caminho para persistir
# comprovantes entre reinícios do container.
RUN mkdir -p /app/wwwroot/uploads

# Usuário não-root por segurança
RUN useradd -m appuser && chown -R appuser:appuser /app
USER appuser

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "Marketplace.Api.dll"]
