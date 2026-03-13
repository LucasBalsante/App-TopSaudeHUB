## App TopSaudeHUB

Aplicacao full stack para gestao de produtos, clientes e pedidos, composta por:

- `backend` em ASP.NET Core 10 com arquitetura em camadas
- `frontend` em Angular 19 com Angular Material
- `db` em PostgreSQL 17
- orquestracao via Docker Compose

O projeto foi organizado para rodar tanto localmente quanto em containers, com o frontend servido por `nginx` em um fluxo semelhante ao de producao e o backend expondo uma API HTTP para consumo do frontend e de clientes como Postman.

## Visao Geral

O sistema cobre tres dominios principais:

- cadastro e consulta de produtos
- cadastro e consulta de clientes
- criacao, consulta e atualizacao de pedidos

No backend, a API usa Minimal APIs, Entity Framework Core para migracoes e persistencia principal, Dapper para consultas pontuais e middleware de idempotencia para operacoes `POST`.

No frontend, a aplicacao Angular consome a API do backend e oferece interfaces para listagem e manipulacao dos recursos principais.

## Stack Tecnologica

### Backend

- ASP.NET Core `net10.0`
- Entity Framework Core
- Npgsql
- Dapper
- Minimal APIs

### Frontend

- Angular 19
- Angular Material
- RxJS

### Infraestrutura

- PostgreSQL 17
- Docker
- Docker Compose
- Nginx

## Arquitetura

### Backend

O backend esta dividido em camadas:

- `src/Api`: ponto de entrada, configuracao, middlewares e endpoints HTTP
- `src/Application`: regras de negocio, DTOs, interfaces e servicos de aplicacao
- `src/Domain`: entidades e enums do dominio
- `src/Infrastructure`: persistencia, repositorios, migracoes, servicos de suporte e seed

Fluxo resumido:

1. `Program.cs` configura CORS, serializacao JSON e carregamento de configuracoes.
2. `Application` registra servicos de negocio.
3. `Infrastructure` registra banco, repositorios e seeding.
4. Middlewares tratam excecoes e idempotencia.
5. Endpoints Minimal API expõem os recursos da aplicacao.

### Frontend

O frontend esta organizado por feature:

- `features/home`
- `features/product`
- `features/customer`
- `core` para constantes, interceptors, tokens e servicos compartilhados
- `styles/components` para componentes visuais reutilizaveis

As rotas principais da interface sao:

- `/` para a pagina inicial
- `/produtos`
- `/clientes`

## Estrutura do Projeto

```text
.
|-- backend/
|   |-- src/
|   |   |-- Api/
|   |   |-- Application/
|   |   |-- Domain/
|   |   `-- Infrastructure/
|   |-- tests/
|   |-- Dockerfile
|   `-- backend.csproj
|-- frontend/
|   |-- src/
|   |-- public/
|   |-- Dockerfile
|   `-- package.json
|-- docker-compose.yml
`-- README.md
```

## Executando com Docker

Esta e a forma mais direta de subir todo o stack.

Antes de subir os containers, crie o arquivo `.env` na raiz do projeto a partir do `.env.example`.

### Subir aplicacao completa

```powershell
docker compose up -d --build
```

### Servicos expostos

- frontend: `http://localhost:4200`
- backend: `http://localhost:8080`
- postgres: `localhost:5432`

### Derrubar containers

```powershell
docker compose down
```

### Derrubar containers e volume do banco

```powershell
docker compose down -v
```

## Executando Localmente

### Backend

Pre-requisitos:

- .NET SDK 10
- PostgreSQL disponivel localmente ou via Docker

Na pasta `backend`:

```powershell
dotnet restore
dotnet run --launch-profile http
```

Para execucao local do backend, mantenha o arquivo `.env` configurado na raiz do repositorio.

### Frontend

Pre-requisitos:

- Node.js 22+
- npm

Na pasta `frontend`:

```powershell
npm install
npm start
```

O Angular sobe em:

- `http://localhost:4200`

## Comportamento do Frontend em Container

O container do frontend roda em modo semelhante ao de producao:

1. executa `npm run build`
2. gera os arquivos em `dist/frontend/browser`
3. copia o build para o `nginx`
4. serve apenas arquivos estaticos

Por isso, alteracoes no codigo-fonte nao aparecem automaticamente no container. Sempre que houver mudancas no frontend, o container precisa ser rebuildado e recriado:

```powershell
docker compose up -d --build frontend
```

## Endpoints Principais da API

### Endpoints de sistema

- `GET /`
- `GET /health`
- `GET /api`

### Produtos

- `GET /api/products/`
- `GET /api/products/{id}`
- `POST /api/products/`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

### Clientes

- `GET /api/customers/`
- `GET /api/customers/{id}`
- `POST /api/customers/`
- `PUT /api/customers/{id}`
- `DELETE /api/customers/{id}`

### Pedidos

- `GET /api/orders/`
- `GET /api/orders/{id}`
- `POST /api/orders/`
- `PUT /api/orders/{id}`

## Idempotencia nas Requisicoes POST

O backend aplica uma regra de idempotencia para operacoes `POST`.

Ao testar criacoes via Postman ou outro cliente HTTP, envie o header:

```text
Idempotency-Key: qualquer-valor-unico
```

Sem esse header, requisicoes `POST` podem retornar erro `400`.

## Scripts Uteis do Frontend

Na pasta `frontend`:

```powershell
npm start
npm run build
npm run watch

## Testes

### Backend

Na pasta `backend`:

```powershell
dotnet test
```

## Configuracao

Atualmente o projeto utiliza configuracoes em:

- `docker-compose.yml`
- `backend/src/Api/appsettings.json`
- `backend/src/Api/appsettings.Container.json`
- `frontend/src/environments/environment.ts`

As credenciais e connection strings do banco e do backend devem ser definidas no arquivo `.env` da raiz. Use `.env.example` como modelo para criar a configuracao local.

## Observacoes Importantes

- o backend usa CORS liberado para facilitar desenvolvimento local
- o banco aplica migracoes automaticamente ao iniciar a API
- o seed inicial popula produtos e clientes quando o banco esta vazio
- o frontend em container usa `nginx`, nao `ng serve`

## Melhorias Recomendadas

- adicionar Swagger ou OpenAPI para exploracao da API
- separar compose de desenvolvimento e compose de producao
- revisar configuracoes locais antes de publicar no GitHub
