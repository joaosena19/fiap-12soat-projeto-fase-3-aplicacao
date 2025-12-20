# Identificação

Aluno: João Pedro Sena Dainese  
Registro FIAP: RM365182  

Turma 12SOAT - Software Architecture  
Grupo individual  
Grupo 15  

Discord: joaodainese  
Email: joaosenadainese@gmail.com  

## Sobre este Repositório

Este repositório contém apenas parte do projeto completo da Fase 3. Para visualizar a documentação completa, diagramas de arquitetura, e todos os componentes do projeto, acesse: [Documentação Completa - Fase 3](https://github.com/joaosena19/fiap-12soat-projeto-fase-3-documentacao)

## Descrição

API REST em .NET para sistema de Oficina Mecânica implementado com Clean Architecture. Gerencia clientes, veículos, serviços e ordens de serviço. Inclui autenticação via Lambda, documentação Swagger e executado em Kubernetes.

## Tecnologias Utilizadas

- **.NET** - Runtime e framework web
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **JWT Authentication** - Autenticação
- **Swagger** - Documentação API
- **Docker** - Containerização
- **Kubernetes** - Orquestração

## Passos para Execução

### 1. Desenvolvimento Local

```bash
cd src
cp .env.example .env
# Editar .env com suas configurações

docker-compose up -d
```

### 2. Execução Direta

```bash
cd src/OficinaMecanica
dotnet restore
dotnet run
# API disponível em http://localhost:5001
```

## Diagramas de Arquitetura

Para visualizar os diagramas de arquitetura e componentes desta aplicação, consulte a documentação completa: [Diagramas de Componentes (C4)](https://github.com/joaosena19/fiap-12soat-projeto-fase-3-documentacao/blob/main/2.%20Diagramas%20de%20Componentes%20(C4)/1_diagrama_de_componentes_c4.md)

## Configuração Docker Compose

- **API**: Porta 5001 (externa) -> 8080 (interna)
- **PostgreSQL**: Porta 5432
- **Health Checks**: Configurados
- **Variables**: Via arquivo `.env`


## Documentação da API

Para consultar a documentação completa dos endpoints da API, acesse: [Documentação de Endpoints](https://github.com/joaosena19/fiap-12soat-projeto-fase-3-documentacao/blob/main/8.%20Endpoints/1_endpoints.md)

## Deploy Kubernetes

```bash
kubectl apply -f k8s/oficina-mecanica-config.yaml
kubectl apply -f k8s/oficina-mecanica-secret.yaml
kubectl apply -f k8s/api/oficina-mecanica-deployment.yaml
```