# Tech Challenge - Checklist Completo (Fase 1)

## 1. Objetivo do Projeto (MVP)
- Entregar o back-end monolítico do Sistema Integrado de Atendimento e Execução de Serviços da oficina.
- Aplicar arquitetura em camadas com conceitos de DDD.
- Garantir qualidade de software e segurança.

---

## 2. Escopo Funcional Obrigatório

### 2.1 Fluxo: Criação da Ordem de Serviço (OS)
- Identificar cliente por CPF/CNPJ.
- ✅ Permitir cadastrar cliente caso não exista.
- ✅ Cadastrar veículo (placa, marca, modelo, ano) vinculado ao cliente.
- Incluir serviços solicitados na OS (ex.: troca de óleo, alinhamento).
- Incluir peças e insumos necessários na OS.
- Gerar orçamento automaticamente com base em serviços + peças/insumos.
- Registrar envio do orçamento para aprovação do cliente.
- Registrar resposta de aprovação/reprovação do cliente.

### 2.2 Fluxo: Acompanhamento da OS
- ✅ Implementar status da OS:
  - ✅ Recebida
  - ✅ Em diagnóstico
  - ✅ Aguardando aprovação
  - ✅ Em execução
  - ✅ Finalizada
  - ✅ Entregue
- ✅ Implementar regras de transição automática de status conforme ações do sistema.
- Disponibilizar endpoint para cliente consultar progresso da OS via API.
- Garantir histórico de mudanças de status (auditoria básica).

### 2.3 Fluxo: Gestão Administrativa
- ✅ CRUD de clientes.
- ✅ CRUD de veículos.
- ✅ CRUD de serviços.
- ✅ CRUD de peças e insumos.
- Controle de estoque (entrada, saída, saldo mínimo opcional).
- Listagem de ordens de serviço com filtros.
- Detalhamento de ordem de serviço.
- Monitoramento do tempo médio de execução dos serviços.

---

## 3. Regras de Negócio e Domínio (DDD)
- Definir Bounded Contexts principais (ex.: Atendimento, Ordem de Serviço, Estoque, Identidade).
- Definir Aggregate Roots (ex.: OrdemDeServico, Cliente, Veículo, Peca/Insumo).
- ✅ Definir Entidades, Value Objects e regras invariantes.
- Definir serviços de domínio/aplicação para orquestração dos fluxos.
- Definir eventos de domínio relevantes (ex.: OrcamentoGerado, OrcamentoAprovado, StatusAlterado).
- Aplicar Linguagem Ubíqua consistente no código e documentação.

---

## 4. Segurança e Validação
- ✅ Implementar autenticação JWT para APIs administrativas.
- ✅ Definir autorização por perfil/regra (ex.: admin, atendente, cliente).
- ✅ Validar CPF/CNPJ (formato e dígitos verificadores).
- ✅ Validar placa do veículo (padrões válidos).
- Validar dados de entrada com mensagens de erro claras.
- ✅ Tratar exceções de forma padronizada (middleware global).
- Evitar exposição de dados sensíveis em logs e respostas.

---

## 5. Arquitetura e Implementação Técnica
- ✅ Manter modelo monolítico em camadas (API, Application, Domain, Infrastructure).
- Revisar separação de responsabilidades entre camadas.
- ✅ Garantir persistência em banco de dados escolhido.
- Justificar tecnicamente a escolha do banco (no README e/ou documento de entrega).
- Garantir APIs RESTful com boas práticas (verbos, status HTTP, versionamento opcional).

---

## 6. Documentação da API e Execução
- ✅ Documentar endpoints com Swagger/OpenAPI.
- Garantir que exemplos de request/response estejam claros.
- ✅ Validar que a documentação sobe junto com a aplicação.

---

## 7. DevOps e Ambiente
- ✅ Validar Dockerfile para build e execução da aplicação.
- ✅ Validar docker-compose.yml para ambiente completo (app + banco e dependências).
- Garantir configuração local simples com poucos comandos.

---

## 8. Qualidade e Testes
- Criar testes unitários para domínios críticos.
- Criar testes de integração para fluxos principais.
- Cobrir no mínimo:
  - Criação da OS com orçamento
  - Aprovação/reprovação de orçamento
  - Transições de status da OS
  - Controle de estoque ao incluir peças/insumos
  - Autenticação/autorização JWT
  - Validações de CPF/CNPJ e placa
- Atingir cobertura mínima de 80% nos domínios críticos.
- ✅ Configurar execução de testes automatizados via comando único.

---

## 9. Relatório de Vulnerabilidades
- Executar scan de vulnerabilidades no código/dependências.
- Consolidar achados (severidade, evidência, impacto).
- Descrever ações de mitigação/correção.
- Incluir resultado no relatório final.

---

## 10. Entregáveis Obrigatórios da Fase 1

### 10.1 Vídeo (até 15 minutos)
- Demonstrar contexto do problema e solução proposta.
- Demonstrar fluxos principais em funcionamento.
- Mostrar autenticação/segurança.
- Mostrar testes executando.
- Mostrar documentação da API e ambiente com Docker.

### 10.2 Documentação DDD (Miro ou equivalente)
- Event Storming completo de:
  - Criação e acompanhamento da OS
  - Gestão de peças e insumos
- Diagramas conforme disciplina de DDD.
- Linguagem Ubíqua aplicada.

### 10.3 Código-fonte no repositório privado
- APIs implementadas conforme requisitos.
- ✅ Dockerfile configurado.
- ✅ docker-compose.yml configurado.
- README.md completo com:
  - Objetivo do projeto
  - Arquitetura e decisões técnicas
  - Pré-requisitos
  - Passo a passo de execução local
  - Como rodar testes
  - Como acessar Swagger

### 10.4 Documento de entrega (PDF)
- Nome do grupo.
- Participantes e usernames no Discord.
- Link da documentação DDD.
- Link do repositório privado.
- Relatório com análise de vulnerabilidades encontradas.

---

## 11. Acesso ao Repositório
- Garantir que o repositório é privado.
- Conceder acesso ao usuário solicitado: soatarchitecture.
- Validar acesso antes da entrega final.

---

## 12. Critérios de Pronto (Definition of Done)
- Todos os requisitos funcionais obrigatórios implementados.
- ✅ Segurança mínima (JWT + validações sensíveis) implementada.
- Cobertura de testes em domínios críticos >= 80%.
- ✅ Docker e docker-compose funcionando em ambiente limpo.
- ✅ Swagger funcionando com endpoints principais.
- Documentação DDD concluída e coerente com o sistema implementado.
- Vídeo gravado e validado.
- PDF final revisado e pronto para submissão.

---

## 13. Plano de Execução Sugerido (ordem prática)
- Etapa 1: Refinar domínio e regras (DDD + linguagem ubíqua).
- Etapa 2: Implementar fluxo ponta a ponta da OS (mínimo funcional).
- Etapa 3: Implementar CRUDs administrativos e estoque.
- ✅ Etapa 4: Implementar autenticação JWT e validações sensíveis.
- Etapa 5: Completar testes unitários/integrados e elevar cobertura.
- Etapa 6: Ajustar Docker, compose, Swagger e README.
- Etapa 7: Executar scan de vulnerabilidades e consolidar relatório.
- Etapa 8: Preparar vídeo, documentação final e PDF de entrega.

---

## 14. Pendências para conferência final (Checklist de submissão)
- Repositório privado com acesso correto.
- README atualizado e testado por alguém do grupo do zero.
- Pipeline/execução local validada em máquina limpa.
- Todas as evidências de testes e cobertura registradas.
- Todas as evidências de segurança e vulnerabilidades registradas.
- Links finais (repo, docs, vídeo) funcionando.
- PDF final revisado ortograficamente e tecnicamente.
