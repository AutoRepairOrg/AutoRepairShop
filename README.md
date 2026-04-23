# AutoRepairShop 🛠️

Projeto backend desenvolvido como **Tech Challenge**, simulando o sistema de gestão de uma oficina mecânica, com foco em **Ordem de Serviço (Service Order)**, domínio rico e boas práticas de arquitetura.

---

## 📐 Arquitetura

O projeto segue uma **Clean Architecture simplificada**, com domínio rico (DDD) e separação clara de responsabilidades:

```
AutoRepairShop
├── src
│   ├── AutoRepairShop.Api              # Camada de apresentação (HTTP / Controllers)
│   ├── AutoRepairShop.Application      # Application Services (casos de uso)
│   ├── AutoRepairShop.Domain           # Domínio (Entidades, VOs, Enums, Regras)
│   └── AutoRepairShop.Infrastructure   # Infraestrutura (EF Core, Repositórios, Migrations)
│
├── tests
│   └── AutoRepairShop.Tests            # Testes unitários e de integração
│
├── docker-compose.yml                  # Orquestração API + SQL Server
└── AutoRepairShop.sln
```

**Padrões adotados:**
- DDD (Domain-Driven Design)
- Aggregate Root (`ServiceOrder`)
- Value Objects
- Repositórios
- DTOs + Mappers
- EF Core

---

## 🧩 Principais Funcionalidades

### Ordem de Serviço (Service Order)
- Criação da OS
- Associação de cliente e veículo
- Inclusão de serviços
- Inclusão de peças/insumos (supplies)
- Geração automática de orçamento
- Fluxo de status:
  - Received
  - InDiagnosis
  - WaitingApproval
  - InExecution
  - Finished
  - Delivered
- Consulta de OS por cliente
- Monitoramento de tempo médio de execução

---

## 🐳 Executando com Docker

### Pré-requisitos
- Docker Desktop
- .NET SDK 8+

---

### 1️⃣ Subir API + Banco de Dados

Na raiz do projeto:

```bash
docker compose up -d --build
```

Isso irá subir:
- **SQL Server 2022** (container)
- **API AutoRepairShop**

---

### 2️⃣ String de Conexão

A API já está configurada para acessar o banco via Docker:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=sqlserver,1433;Database=AutoRepairShopDb;User Id=sa;Password=StrongPassword@123;TrustServerCertificate=True"
}
```

> ⚠️ Importante: o nome do servidor é `sqlserver` (nome do serviço no docker-compose)

---

## 🗄️ Migrations (EF Core)

As migrations ficam no projeto:

```
src/AutoRepairShop.Infrastructure
```

### Criar uma migration

```bash
dotnet ef migrations add InitialCreate \
  --project src/AutoRepairShop.Infrastructure \
  --startup-project src/AutoRepairShop.Api
```

### Aplicar migrations

```bash
dotnet ef database update \
  --project src/AutoRepairShop.Infrastructure \
  --startup-project src/AutoRepairShop.Api
```

> 💡 O banco precisa estar rodando (via Docker)

---

## 🧪 Testes

### Tipos de testes implementados
- Testes unitários do domínio (entidades e regras)
- Testes de Application Services
- Testes de integração com banco

### Executar testes

```bash
dotnet test
```

### Cobertura
- Cobertura mínima exigida: **80% nos domínios críticos**
- Foco em `ServiceOrder` e fluxos principais

---

## 📦 Tecnologias Utilizadas

- .NET 8
- ASP.NET Core
- Entity Framework Core
- SQL Server 2022
- Docker & Docker Compose
- xUnit / FluentAssertions / Moq

---


## 🚀 Status do Projeto

✔ Estrutura definida
✔ Domínio modelado
✔ Docker configurado
✔ Migrations funcionando
✔ Aggregate implementado
⬜ Endpoints REST
⬜ Testes finais

---

## 👩‍💻 Autores

Projeto desenvolvido por **Dhiulia da Silva e Mateus Pinheiro** como parte de avaliação técnica.

---


