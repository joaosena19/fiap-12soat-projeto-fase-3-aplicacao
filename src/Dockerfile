# Estágio base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Estágio de build 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["OficinaMecanica/API.csproj", "OficinaMecanica/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "OficinaMecanica/API.csproj"

COPY . .

WORKDIR "/src/OficinaMecanica"
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Instalar dependências básicas
USER root
RUN apt-get update && apt-get install -y wget tar

# Baixar e instalar New Relic manualmente dentro da pasta /app
# Isso garante que não teremos problemas de permissão de pasta
RUN mkdir -p /app/newrelic && \
    wget -O /tmp/newrelic-dotnet-agent.tar.gz https://download.newrelic.com/dot_net_agent/latest_release/newrelic-dotnet-agent_linux_x64.tar.gz && \
    tar -xf /tmp/newrelic-dotnet-agent.tar.gz -C /app/newrelic --strip-components=1 && \
    rm /tmp/newrelic-dotnet-agent.tar.gz

# Dar permissão total para o agente do newrelic
RUN chmod -R 777 /app/newrelic

# Variáveis de ambiente para o New Relic default (O Kubernetes pode sobrescrever)
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_PROFILER_PATH=/app/newrelic/libNewRelicProfiler.so \
    NEW_RELIC_HOME=/app/newrelic

# Copia a aplicação
COPY --from=build /app/publish .

# Volta para o usuário padrão de segurança
USER $APP_UID

ENTRYPOINT ["dotnet", "API.dll"]