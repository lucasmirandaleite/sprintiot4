ADVANCED BUSINESS DEVELOPMENT WITH .NET - Mottu RFID API

Este projeto consiste em uma API RESTful desenvolvida em .NET para rastreamento de motos via RFID, com foco em **boas práticas de desenvolvimento REST** e a implementação de **Machine Learning (ML.NET)** para predição de localização e análise de padrões de movimento.

## 📋 Sobre o Projeto

Sistema de rastreamento de motos via RFID desenvolvido em .NET 9.0, implementando uma API RESTful completa com funcionalidades avançadas de monitoramento, machine learning e segurança.

## 👥 Integrantes da Equipe

*   Lucas Miranda Leite RM:555161
*   Gusthavo Daniel De Souza RM:554681
*   Guilherme Damasio Roselli RM:555873

## 🏗️ Arquitetura do Sistema

O projeto segue a arquitetura **Clean Architecture** com as seguintes camadas:

*   `MottuRFID.API`: Camada de apresentação (Controllers, Middleware)
*   `MottuRFID.Application`: Camada de aplicação (DTOs, Services)
*   `MottuRFID.Domain`: Camada de domínio (Entities, Interfaces)
*   `MottuRFID.Infrastructure`: Camada de infraestrutura (Data, Repositories)
*   `MottuRFID.Tests`: Camada de testes (Unitários e Integração)

## 🚀 Funcionalidades Implementadas

O projeto implementa as seguintes funcionalidades avançadas:

*   **Boas Práticas REST:** API RESTful completa com 3 entidades principais (Motos, Filiais, Pontos de Leitura), endpoints CRUD com paginação, filtros e uso de verbos HTTP corretos.
*   **Documentação Swagger/OpenAPI:** Documentação Swagger/OpenAPI completa com exemplos e modelos.
*   **Repositório GitHub:** Repositório GitHub com README detalhado.
*   **Segurança de API (API KEY):** Implementação de segurança via **API Key** para proteção de endpoints.
*   **ML.NET:** Implementação de endpoints que utilizam **ML.NET** para predição de localização e análise de padrões de movimento.
*   **Testes Unitários (xUnit):** Cobertura de testes unitários com **xUnit** e **Moq** para a lógica principal do domínio e aplicação.
*   **Testes de Integração:** Testes de integração básicos utilizando **WebApplicationFactory** para validar o fluxo completo da API.
*   **Health Checks:** Implementação de um endpoint de **Health Checks** para monitoramento do status da aplicação e serviços dependentes.
*   **Versionamento da API:** Implementação de **versionamento da API** para gerenciamento de diferentes versões de endpoints.



## 🔧 Tecnologias Utilizadas

*   **.NET 9.0** - Framework principal
*   **Entity Framework Core** - ORM para acesso a dados
*   **Oracle Database** - Banco de dados principal (configuração comentada)
*   **Swagger/OpenAPI** - Documentação da API
*   **ML.NET** - Machine Learning
*   **xUnit** - Framework de testes unitários e de integração
*   **Moq** - Mock para testes unitários
*   **WebApplicationFactory** - Suporte para testes de integração

## 📊 Endpoints da API

A API está versionada, sendo o prefixo `v1` o padrão atual.

### Versionamento

A API utiliza o versionamento por URL. Exemplo: `/api/v1/motos`.

### Health Checks

*   `GET /api/health` - Status completo da aplicação e serviços dependentes (banco de dados, etc.).
*   `GET /api/health/ping` - Verificação básica de funcionamento.

### Motos

*   `GET /api/v1/motos` - Listar motos com filtros
*   `GET /api/v1/motos/{id}` - Buscar moto por ID
*   `GET /api/v1/motos/tag/{tagRFID}` - Buscar moto por tag RFID
*   `POST /api/v1/motos` - Criar nova moto
*   `PUT /api/v1/motos/{id}` - Atualizar moto
*   `DELETE /api/v1/motos/{id}` - Excluir moto

### Filiais

*   `GET /api/v1/filiais` - Listar filiais
*   `GET /api/v1/filiais/{id}` - Buscar filial por ID
*   `GET /api/v1/filiais/{id}/motos` - Listar motos da filial
*   `POST /api/v1/filiais` - Criar nova filial
*   `PUT /api/v1/filiais/{id}` - Atualizar filial
*   `DELETE /api/v1/filiais/{id}` - Excluir filial

### Pontos de Leitura

*   `GET /api/v1/pontosleitura` - Listar pontos de leitura
*   `GET /api/v1/pontosleitura/{id}` - Buscar ponto por ID
*   `POST /api/v1/pontosleitura` - Criar novo ponto
*   `PUT /api/v1/pontosleitura/{id}` - Atualizar ponto
*   `DELETE /api/v1/pontosleitura/{id}` - Excluir ponto

### RFID

*   `POST /api/v1/rfid/registrar` - Registrar leitura RFID
*   `GET /api/v1/rfid/historico/moto/{id}` - Histórico da moto
*   `GET /api/v1/rfid/localizacao/filial/{id}` - Localização das motos

### Machine Learning

*   `GET /api/v1/ml/PredictNextLocation/{motoId}` - Predizer próxima localização
*   `GET /api/v1/ml/AnalyzeMovementPatterns/{filialId}` - Analisar padrões de movimento

## 🔐 Autenticação

A API utiliza autenticação via **API Key**. Para acessar os endpoints protegidos, inclua o header:

`X-API-Key: mottu-rfid-api-key-2024`

**Observação:** Em um ambiente de produção, a chave deve ser gerenciada de forma segura (e.g., variáveis de ambiente ou Key Vault).

## 🗄️ Configuração do Banco de Dados

Configure a string de conexão no `appsettings.json`:

\`\`\`json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HOST:PORT/SID;User Id=USER;Password=PASS;"
  }
}
\`\`\`

**Observação:** A configuração do banco de dados Oracle está comentada no `Program.cs`. Para utilizá-la, descomente as linhas e configure a string de conexão no `appsettings.json`.

## 🚀 Como Executar

### Pré-requisitos

*   .NET 9.0 SDK
*   Oracle Database (opcional, se descomentar a configuração)

### Passos para Execução

1.  Navegue até o diretório principal do projeto:
    \`\`\`bash
    cd /caminho/para/o/projeto/Mottu.NET
    \`\`\`
2.  Restaure as dependências:
    \`\`\`bash
    dotnet restore
    \`\`\`
3.  Compile o projeto:
    \`\`\`bash
    dotnet build
    \`\`\`
4.  Execute a API:
    \`\`\`bash
    dotnet run --project MottuRFID.API/MottuRFID.API.csproj
    \`\`\`
    A API será iniciada e estará disponível em `http://localhost:5193` (ou outra porta configurada, verifique o console).

### Acessando o Swagger/OpenAPI

Após iniciar a API, você pode acessar a documentação interativa do Swagger/OpenAPI através do seguinte URL:

`http://localhost:5193/swagger`

O Swagger UI fornece:

*   Descrição de endpoints e parâmetros: Detalhes sobre cada rota da API, métodos HTTP, parâmetros de entrada e saída.
*   Exemplos de payloads: Modelos de dados para requisições e respostas.
*   Modelos de dados descritos: Definições dos objetos utilizados na API.

## 🧪 Executar Testes

O projeto inclui testes unitários para a lógica de domínio e testes de integração básicos para validar o fluxo da API.

### Testes Unitários e de Integração

Os testes estão localizados no projeto `MottuRFID.Tests` e utilizam **xUnit** como framework de testes, **Moq** para simulação de dependências e **WebApplicationFactory** para os testes de integração.

Para rodar todos os testes (unitários e de integração):

\`\`\`bash
dotnet test MottuRFID.Tests/MottuRFID.Tests.csproj
\`\`\`

O resultado da execução indicará a quantidade de testes executados, aprovados e falhados.

### Cobertura de Testes

Recomenda-se o uso de ferramentas como **coverlet** para gerar relatórios de cobertura de código.

1.  Instale a ferramenta globalmente (se ainda não estiver instalada):
    \`\`\`bash
    dotnet tool install -g dotnet-reportgenerator-globaltool
    \`\`\`
2.  Execute os testes com a coleta de cobertura:
    \`\`\`bash
    dotnet test MottuRFID.Tests/MottuRFID.Tests.csproj --collect:"XPlat Code Coverage" --DataCollectionRunSettings.Settings.xml=coverlet.runsettings
    \`\`\`
3.  Gere o relatório de cobertura (opcional):
    \`\`\`bash
    reportgenerator "-reports:MottuRFID.Tests/TestResults/*/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html
    \`\`\`
    O relatório HTML estará disponível no diretório `coveragereport`.
