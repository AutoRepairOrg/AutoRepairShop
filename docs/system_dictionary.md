# 📘 Ubiquitous Language — AutoRepairShop

Este documento define a **Linguagem Ubíqua** do sistema **AutoRepairShop**.  
Os termos aqui descritos devem ser usados de forma consistente em **código, APIs, banco de dados, testes e comunicação**.

---

## 🧩 Bounded Contexts

### 🔧 Ordem de Serviço
Responsável pelo ciclo completo da Ordem de Serviço: criação, diagnóstico, orçamento, execução e entrega.

### 📦 Gestão de Peças e Insumos
Responsável pelo cadastro, controle de estoque e precificação de peças e insumos utilizados nas ordens de serviço.

---

## 👥 Atores do Domínio

### Admin
Usuário do sistema responsável pelas operações administrativas.

Responsabilidades:
- Cadastrar clientes
- Cadastrar veículos
- Cadastrar serviços
- Criar e gerenciar ordens de serviço
- Gerenciar peças e insumos

> O Admin pode representar atendente, mecânico ou gestor.

---

## 🧑‍💼 Entidades do Domínio

### Customer (Cliente)
Pessoa física ou jurídica que solicita serviços automotivos.

**Características:**
- Possui um documento (CPF ou CNPJ)
- Pode possuir vários veículos
- Pode possuir várias ordens de serviço

---

### Vehicle (Veículo)
Veículo pertencente a um cliente.

**Características:**
- Pertence a um único cliente
- Identificado por placa
- Pode ter várias ordens de serviço ao longo do tempo

---

### Service (Serviço)
Serviço oferecido pela oficina.

**Exemplos:**
- Troca de óleo
- Alinhamento
- Balanceamento

**Características:**
- Possui preço fixo
- Pode ser utilizado em várias ordens de serviço

---

### ServiceOrder (Ordem de Serviço)
Agregado raiz que representa uma solicitação de serviço.

**Características:**
- Pertence a um cliente
- Refere-se a um veículo
- Possui um serviço principal
- Possui itens (peças/insumos)
- Possui status controlado
- Possui valor total calculado automaticamente

---

### Supply (Peça / Insumo)
Item físico utilizado na execução de um serviço.

**Exemplos:**
- Óleo
- Filtro
- Correia

**Características:**
- Possui estoque
- Possui preço unitário
- Pode ser utilizado em várias ordens de serviço

---

## 📦 Value Objects

### Document
Representa um documento de identificação.

**Tipos:**
- CPF
- CNPJ

**Regras:**
- Deve ser válido conforme o tipo
- É imutável

---

### VehiclePlate
Representa a placa de um veículo.

**Regras:**
- Deve seguir o padrão nacional
- É imutável

---

### ServiceOrderItem
Representa uma peça ou insumo dentro de uma ordem de serviço.

**Regras:**
- Não existe fora de uma ordem de serviço
- Não possui identidade própria
- É definido por:
  - Peça/Insumo
  - Quantidade
  - Preço unitário

---

## 🔄 Status da Ordem de Serviço

### ServiceOrderStatus

- Received — Ordem criada
- InDiagnosis — Veículo em diagnóstico
- WaitingApproval — Orçamento aguardando aprovação
- InExecution — Serviço em execução
- Finalized — Serviço concluído
- Delivered — Veículo entregue ao cliente
- Canceled — Serviço cujo orçamento foi recusado pelo cliente

**Regras:**
- O status segue uma ordem lógica
- Algumas ações só podem ocorrer em determinados status

---

## 💰 Orçamento

O orçamento é calculado automaticamente com base em:
- Preço do serviço
- Soma dos itens (peças e insumos)

---

## ⏱️ Tempo Médio de Execução

Métrica utilizada para acompanhamento operacional.

**Cálculo:**
- Tempo entre início da execução e finalização da ordem

Utilizado para:
- Relatórios
- Indicadores de desempenho

---
