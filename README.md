# AutoRepairShop 🛠️

Projeto backend desenvolvido como **Tech Challenge**, simulando o sistema de gestão de uma oficina mecânica, com foco em **Ordem de Serviço (Service Order)**, domínio rico e boas práticas de arquitetura.

---

## 📐 Arquitetura

O projeto segue uma **Clean Architecture simplificada**, com princípios de **DDD (Domain-Driven Design)** e separação clara de responsabilidades:


```
AutoRepairShop
├── src
│ ├── AutoRepairShop.Api # Camada de apresentação (HTTP / Controllers)
│ ├── AutoRepairShop.Application # Application Services (casos de uso)
│ ├── AutoRepairShop.Domain # Domínio (Entidades, VOs, Enums, Regras)
│ └── AutoRepairShop.Infrastructure # Infraestrutura (EF Core, Repositórios, Migrations)
│
├── tests
│ └── AutoRepairShop.Tests # Testes unitários e de integração
│
├── docker-compose.yml # Orquestração API + SQL Server
├── .env.example # Exemplo de variáveis de ambiente
└── AutoRepairShop.sln
```

### Padrões adotados
- Domain-Driven Design (DDD)
- Aggregate Root (`ServiceOrder`)
- Value Objects
- Repositórios
- DTOs + Mappers
- Entity Framework Core
- Separação por camadas

---

## 🧩 Principais Funcionalidades

### Ordem de Serviço (Service Order)
- Criação de Ordem de Serviço
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
  - Canceled
- Consulta de OS por cliente
- Monitoramento de tempo médio de execução

---

## 🐳 Executando com Docker

### Pré-requisitos
- Docker Desktop
- .NET SDK 8+

---

### 1️⃣ Configurar variáveis de ambiente

Crie um arquivo `.env` na raiz do projeto (não versionado):

```env
DB_PASSWORD=your_strong_password_here

JWT_KEY=your_super_secret_jwt_key_with_256_bits_or_more
JWT_ISSUER=AutoRepairShop
JWT_AUDIENCE=AutoRepairShopUsers
JWT_EXPIRES_IN_MINUTES=15
```
---

### 2️⃣ Subir API + Banco de Dados

Na raiz do projeto:

```bash
docker compose up -d --build
```

Isso irá subir:
- **SQL Server 2022** (container)
- **API AutoRepairShop**

---

🔌 String de Conexão

A aplicação suporta execução fora do Docker, enquanto o banco roda em container.

Execução local da API (banco no Docker)

````json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=AutoRepairShopDb;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=True"
}
````

Execução da API dentro do Docker

````json
"ConnectionStrings": {
  "DefaultConnection": "Server=sqlserver,1433;Database=AutoRepairShopDb;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=True"
}
````

⚠️ sqlserver é o nome do serviço definido no docker-compose.yml.

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

## 🔐 Autenticação JWT

A aplicação utiliza JWT (JSON Web Token) para autenticação.

Configuração segura

Nenhuma secret é versionada.
As configurações são lidas via variáveis de ambiente.

````json
"Jwt": {
  "Key": "${JWT_KEY}",
  "Issuer": "${JWT_ISSUER}",
  "Audience": "${JWT_AUDIENCE}",
  "ExpiresInMinutes": "${JWT_EXPIRES_IN_MINUTES}"
}
````

Variáveis injetadas no Docker

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
✔ Endpoints REST
✔ Testes finais

---

## 👩‍💻 Autores

Projeto desenvolvido por **Dhiulia da Silva e Mateus Pinheiro** como parte de avaliação técnica.

---


