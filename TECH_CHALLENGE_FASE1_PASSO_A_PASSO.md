# Tech Challenge - Passo a Passo de Execucao (Fase 1)

Este arquivo e o guia operacional do time.
Conforme cada tarefa for concluida, atualize o checklist principal em [TECH_CHALLENGE_FASE1_CHECKLIST.md](TECH_CHALLENGE_FASE1_CHECKLIST.md) marcando com ✅ apenas o que estiver totalmente pronto.

---

## Regra de Atualizacao
- Neste arquivo: usar os checkboxes [ ] e [x] para controlar andamento da execucao.
- No checklist principal: marcar ✅ somente quando a entrega estiver finalizada e validada.
- Nao marcar parcialmente.

---

## Etapa 1 - Fechar fluxo de Ordem de Servico (OS)

### 1.1 Criacao da OS
- [ ] Criar endpoint de criacao da OS no controller.
- [ ] Receber cliente por CPF/CNPJ (buscar existente, criar se necessario).
- [ ] Vincular veiculo ao cliente.
- [ ] Permitir incluir servicos solicitados.
- [ ] Permitir incluir pecas/insumos.
- [ ] Calcular orcamento automaticamente.
- [ ] Persistir OS com status inicial.

Critério de pronto:
- [ ] OS criada com sucesso via API.
- [ ] Valor total do orcamento consistente com servicos + pecas.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 2.1 (itens de criacao e orcamento).

### 1.2 Acompanhamento da OS
- [ ] Criar endpoint de consulta de progresso da OS para cliente.
- [ ] Expor status atual da OS.
- [ ] Expor dados basicos (cliente, veiculo, itens, total).
- [ ] Registrar historico de mudancas de status (auditoria basica).

Critério de pronto:
- [ ] Cliente consegue consultar progresso por API.
- [ ] Historico de status gravado e consultavel.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 2.2 (consulta e historico).

---

## Etapa 2 - Gestao administrativa avancada

### 2.1 Estoque e pecas/insumos
- [ ] Definir regra de baixa de estoque ao incluir item na OS.
- [ ] Impedir estoque negativo.
- [ ] Criar regra de validacao para quantidade disponivel.
- [ ] Atualizar saldo de estoque de forma consistente.

Critério de pronto:
- [ ] Fluxo de estoque funciona em cenario feliz e cenario de erro.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 2.3 (controle de estoque).

### 2.2 Listagem e detalhamento de OS
- [ ] Criar endpoint de listagem de OS com filtros (status, cliente, periodo).
- [ ] Criar endpoint de detalhamento por id.
- [ ] Implementar retorno paginado (se aplicavel).

Critério de pronto:
- [ ] Lista e detalhe funcionando e testados.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 2.3 (listagem e detalhamento de OS).

### 2.3 Tempo medio de execucao
- [ ] Definir metrica (ex.: InExecution ate Finished).
- [ ] Implementar consulta de tempo medio por periodo/servico.
- [ ] Expor endpoint administrativo.

Critério de pronto:
- [ ] Endpoint retorna metrica correta com base em dados reais.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 2.3 (monitoramento do tempo medio).

---

## Etapa 3 - Refino de DDD e qualidade arquitetural

### 3.1 Dominio e linguagem ubiqua
- [ ] Revisar aggregate roots e limites de contexto.
- [ ] Consolidar termos da linguagem ubiqua no codigo.
- [ ] Documentar eventos de dominio importantes.

Critério de pronto:
- [ ] Dominio descrito de forma clara e consistente com o codigo.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 3 (itens nao concluidos).

### 3.2 Boas praticas de API e validacoes
- [ ] Padronizar codigos HTTP e mensagens de erro.
- [ ] Revisar validacoes de entrada de DTOs.
- [ ] Revisar protecao de dados sensiveis em logs e respostas.

Critério de pronto:
- [ ] Respostas de erro consistentes e sem vazamento de dados sensiveis.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 4 (validacao de dados e logs).
- [ ] Secao 5 (boas praticas REST).

---

## Etapa 4 - Testes e cobertura

### 4.1 Testes unitarios criticos
- [ ] Criar testes para dominio de OS (transicoes de status).
- [ ] Criar testes para calculo de orcamento.
- [ ] Criar testes para regras de estoque.
- [ ] Criar testes para validacoes CPF/CNPJ e placa.

### 4.2 Testes de integracao
- [ ] Criar testes para fluxo ponta a ponta da OS.
- [ ] Criar testes para autenticacao/autorizacao JWT.
- [ ] Criar testes de endpoints de listagem/detalhe.

### 4.3 Cobertura
- [ ] Medir cobertura dos dominios criticos.
- [ ] Subir para >= 80%.

Critério de pronto:
- [ ] Suite de testes verde.
- [ ] Cobertura >= 80% nos dominios criticos.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 8 completa.
- [ ] Secao 12 (cobertura).

---

## Etapa 5 - Documentacao tecnica e operacao local

### 5.1 README
- [ ] Escrever objetivo do projeto.
- [ ] Descrever arquitetura e decisoes tecnicas.
- [ ] Documentar pre-requisitos.
- [ ] Documentar execucao local.
- [ ] Documentar execucao de testes.
- [ ] Documentar acesso ao Swagger.
- [ ] Justificar escolha do banco.

### 5.2 Swagger
- [ ] Revisar exemplos de request/response.
- [ ] Garantir documentacao dos endpoints novos.

Critério de pronto:
- [ ] Um integrante roda o projeto do zero apenas com README.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 5 (justificativa do banco).
- [ ] Secao 6 (exemplos request/response).
- [ ] Secao 7 (configuracao local simples).
- [ ] Secao 10.3 (README completo).

---

## Etapa 6 - Seguranca e vulnerabilidades

### 6.1 Analise de vulnerabilidades
- [ ] Rodar scan de dependencias.
- [ ] Registrar achados por severidade.
- [ ] Corrigir vulnerabilidades prioritarias.
- [ ] Reexecutar scan para evidenciar melhoria.

Critério de pronto:
- [ ] Relatorio de vulnerabilidades fechado com evidencias.

Quando concluir, atualizar no checklist principal:
- [ ] Secao 9 completa.

---

## Etapa 7 - Entregaveis finais (fora do codigo)

### 7.1 DDD e apresentacao
- [ ] Finalizar Event Storming.
- [ ] Finalizar diagramas DDD.
- [ ] Gravar video (ate 15 min).

### 7.2 Documento de entrega
- [ ] Montar PDF com dados do grupo.
- [ ] Inserir links (repositorio, documentacao, relatorio).
- [ ] Revisao final.

### 7.3 Repositorio
- [ ] Confirmar repositorio privado.
- [ ] Conceder acesso a soatarchitecture.
- [ ] Validar acesso.

Critério de pronto:
- [ ] Todos os links funcionando e arquivos finais prontos para submissao.

Quando concluir, atualizar no checklist principal:
- [ ] Secoes 10, 11, 12 e 14.

---

## Ritual semanal (recomendado)
- [ ] Segunda: priorizar 3 entregas tecnicas da semana.
- [ ] Quarta: revisar bloqueios e redistribuir tarefas.
- [ ] Sexta: atualizar este arquivo e refletir ✅ no checklist principal.
- [ ] Registrar no commit/PR quais itens foram concluidos.
