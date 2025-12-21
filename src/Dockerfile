# Estágio base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Estágio de build 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia todos os arquivos .csproj e restaura as dependências primeiro
# Isso otimiza o cache do Docker
COPY ["OficinaMecanica/API.csproj", "OficinaMecanica/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "OficinaMecanica/API.csproj"

# Copia todo o resto do código-fonte
COPY . .

# Publica a aplicação
WORKDIR "/src/OficinaMecanica"
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Instalação do Agente New Relic (compatível com Debian 12)
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
    && wget -O- https://download.newrelic.com/548C16BF.gpg | gpg --dearmor -o /usr/share/keyrings/newrelic-archive-keyring.gpg \
    && echo 'deb [signed-by=/usr/share/keyrings/newrelic-archive-keyring.gpg] http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
    && apt-get update \
    && apt-get install -y newrelic-dotnet-agent

# Habilitar o Profiler do New Relic no Ambiente
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    NEW_RELIC_HOME=/usr/local/newrelic-dotnet-agent

# Copia a aplicação
COPY --from=build /app/publish .

# Entrypoint correto para o seu projeto
ENTRYPOINT ["dotnet", "API.dll"]