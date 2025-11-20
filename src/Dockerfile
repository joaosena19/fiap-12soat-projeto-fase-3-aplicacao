# Estágio base (sem alterações)
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
# Adicione outros projetos .csproj se houver
RUN dotnet restore "OficinaMecanica/API.csproj"

# Copia todo o resto do código-fonte
COPY . .

# Publica a aplicação
WORKDIR "/src/OficinaMecanica"
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final (sem alterações)
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]