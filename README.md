# Marketplace Backend

API REST do MVP de Marketplace (modelo Mercado Livre, fluxo financeiro manual).
Construída em **.NET 9** com arquitetura **DDD em camadas**
(`01-Api / 02-Domain / 03-Infra`), persistência em **PostgreSQL** via Dapper e
autenticação **JWT + BCrypt**.

---

## 1. Stack

| Camada | Tecnologia |
|---|---|
| Runtime | .NET 9 (`net9.0`) |
| HTTP | ASP.NET Core Controllers + Swashbuckle |
| Persistência | **PostgreSQL** (Npgsql + Dapper) |
| Migrações | **DbUp** (scripts SQL como Embedded Resources, executados no startup) |
| Validação | FluentValidation |
| Logging | `ILogger<T>` built-in |
| Auth | **JWT HS256** (`Microsoft.AspNetCore.Authentication.JwtBearer`) + senha em **BCrypt** |

## 2. Estrutura de pastas

```
marketplace-backend/
├── Marketplace.sln
├── 01-Api/Marketplace.Api/                  # Host ASP.NET Core (Controllers, Middlewares, Program.cs)
├── 02-Domain/
│   ├── Marketplace.Domain/                  # Models, Enums, Interfaces (Repository + Service), Settings (Jwt/Platform/ConnectionStrings)
│   └── Marketplace.Application/             # UseCases, DTOs, Validators, Exceptions
└── 03-Infra/
    ├── Marketplace.Setup/                   # Composition root (IServiceCollectionExtensions.IocConfiguration)
    ├── Marketplace.Repository/              # Data/ (NpgsqlConnectionFactory + DatabaseMigrator) · Migrations/*.sql · Postgres/*Repository
    └── Marketplace.Infrastructure/          # Auth/ (JwtService, CurrentUserResolver) · Shipping/MockShippingCalculator · Storage/LocalFileStorageService
```

## 3. Como rodar

### Requisitos

- .NET SDK 9.
- Um Postgres acessível (Render, Neon, RDS, ou local via `docker run postgres`).

### Configuração

Em [`01-Api/Marketplace.Api/appsettings.Development.json`](01-Api/Marketplace.Api/appsettings.Development.json)
já vem preenchido o Postgres gerenciado do Render usado no desenvolvimento
desta iteração. Sobrescreva conforme necessário:

```jsonc
{
  "ConnectionStrings": {
    "Postgres": "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"
  },
  "JwtSettings": {
    "SigningKey": "min-32-chars-please-change-me",
    "Issuer": "marketplace-api",
    "Audience": "marketplace-web",
    "ExpiresHours": 8
  }
}
```

Em produção use variáveis de ambiente equivalentes:
`ConnectionStrings__Postgres`, `JwtSettings__SigningKey`, etc. — já
espelhadas no [`docker-compose.yml`](docker-compose.yml).

### Executando

```powershell
cd marketplace-backend
dotnet restore
dotnet run --project 01-Api/Marketplace.Api --launch-profile http
```

- API: `http://localhost:5066`
- Swagger UI: `http://localhost:5066/swagger`
- CORS já habilitado para `http://localhost:5173` (Vite dev) e `http://localhost:4173` (Vite preview).

Na primeira execução o [`DatabaseMigrator`](03-Infra/Marketplace.Repository/Data/DatabaseMigrator.cs)
aplica `001_init_schema.sql` (cria tabelas) e `002_seed_data.sql` (semeia
3 usuários / 4 categorias / 6 produtos / 2 pedidos / 1 repasse). Os scripts
são idempotentes: subir novamente já não reprocessa nada.

### Integrando com o frontend

O frontend ([marketplace-frontend](../marketplace-frontend)) chama
`baseURL: '/api'` em [`src/lib/axios.ts`](../marketplace-frontend/src/lib/axios.ts)
e já tem um `server.proxy` no `vite.config.ts` apontando para `http://localhost:5066`.
Basta rodar `npm run dev` no frontend depois de subir a API.

## 4. Credenciais de teste

Os 3 usuários abaixo (senha `123456`, hash BCrypt pré-gerado) são semeados
na migration `002_seed_data.sql`:

| Role | E-mail | Senha |
|---|---|---|
| Buyer | `comprador@teste.com` | `123456` |
| Seller | `vendedor@teste.com` | `123456` |
| Admin | `admin@teste.com` | `123456` |

Para autenticar chame `POST /api/auth/login` e cole o `token` retornado no
botão "Authorize" do Swagger (sem o prefixo `Bearer`). Em Postman/curl:
`Authorization: Bearer <jwt>`.

## 5. Endpoints (27 rotas, todas sob `/api`)

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| POST | `/api/auth/login` | público | `{email,password}` → `{user,token(JWT)}` |
| POST | `/api/auth/register` | público | Cria conta buyer (201) |
| GET  | `/api/auth/me` | Bearer | User atual |
| GET  | `/api/products` | público | `?q=&categoryId=&includeSubcategories=` |
| GET  | `/api/products/:id` | público | 404 se não encontrar |
| GET  | `/api/categories` | público | Árvore plana |
| POST | `/api/orders` | Bearer | `{productId,quantity,address,shippingCost}` → `{order, pixKey}` (201) |
| POST | `/api/orders/:id/receipt` | Bearer (dono) | `multipart/form-data` campo `receipt` |
| GET  | `/api/orders/me` | Bearer | Histórico do buyer |
| POST | `/api/shipping/quote` | público | `{cepOrigem,cepDestino,peso}` (mock) |
| GET  | `/api/shipping/address/:cep` | público | Proxy ViaCEP (`{cep,street,neighborhood,city,state,complement?}`) |
| GET  | `/api/seller/products` | Seller | Produtos do vendedor logado |
| POST | `/api/seller/products` | Seller | Cria anúncio (201) |
| PUT  | `/api/seller/products/:id` | Seller | Atualização parcial |
| DELETE | `/api/seller/products/:id` | Seller | `{success:true}` |
| GET  | `/api/seller/sales` | Seller | Pedidos com status `Pago/Enviado/Entregue` |
| PATCH | `/api/seller/sales/:id/tracking` | Seller | `{trackingCode}` → status vira `Enviado` |
| POST | `/api/seller/onboarding` | Seller | Completa cadastro (CPF/CNPJ, Pix, CEP origem) |
| GET  | `/api/admin/users` | Admin | Lista todos os usuários |
| PATCH | `/api/admin/users/:id/ban` | Admin | `{banned:boolean}` |
| GET  | `/api/admin/categories` | Admin | — |
| POST | `/api/admin/categories` | Admin | `{name, parentId?}` (201) |
| PUT  | `/api/admin/categories/:id` | Admin | — |
| DELETE | `/api/admin/categories/:id` | Admin | `{success:true}` |
| GET  | `/api/admin/orders` | Admin | Pedidos `Em Análise` aguardando aprovação |
| POST | `/api/admin/orders/:id/approve` | Admin | Decrementa estoque, cria `Repasse` (10% comissão), status vira `Pago` |
| GET  | `/api/admin/repasses` | Admin | Lista repasses (pagos e pendentes) |
| POST | `/api/admin/repasses/:id/mark-paid` | Admin | Marca repasse como pago |

### Contratos preservados (paridade total com o frontend)

- `OrderStatus` serializado em **PT-BR com acentos literais**: `"Aguardando Comprovante"`, `"Em Análise"`, `"Pago"`, `"Enviado"`, `"Entregue"`.
- `UserRole` em minúsculas: `"buyer"`, `"seller"`, `"admin"`.
- Preços e pesos como `decimal` em reais.
- Datas em ISO-8601 UTC.
- Erros sempre `{ "message": "..." }` em português (400 / 401 / 403 / 404 / 502).
- JSON em **camelCase** com campos `null` explícitos.

## 6. Regras de negócio relevantes

- **Aprovar pedido (`POST /api/admin/orders/:id/approve`)** debita o estoque do produto,
  muda o pedido para `Pago` e cria um `Repasse` com `commission = productPrice * CommissionRate`
  (default 10%, configurável em `appsettings.json::PlatformSettings.CommissionRate`) e
  `netAmount = productPrice + shippingCost - commission`.
- **`GET /api/seller/sales`** filtra somente status `Pago`, `Enviado` ou `Entregue`.
- **`GET /api/admin/orders`** retorna somente status `Em Análise`.
- **Criar pedido (`POST /api/orders`)** devolve `{ order, pixKey }`, onde `pixKey`
  vem de `PlatformSettings.PixKey` (default `marketplace@pix.com.br`).
- **`GET /api/products?includeSubcategories=true`** inclui produtos das
  categorias filhas da categoria selecionada (usado pela home ao clicar em
  um departamento com subcategorias).

## 7. Persistência (PostgreSQL)

Tabelas gerenciadas em [`03-Infra/Marketplace.Repository/Migrations/`](03-Infra/Marketplace.Repository/Migrations)
via [DbUp](https://dbup.readthedocs.io/):

- `001_init_schema.sql` — `users`, `seller_profiles`, `categories`,
  `products`, `product_images`, `orders`, `repasses`.
- `002_seed_data.sql` — dados demo (3 usuários com senha `123456` já em
  BCrypt, 4 categorias, 6 produtos, 2 pedidos, 1 repasse).

Ao adicionar novos scripts, use o próximo prefixo numérico (`003_...`,
`004_...`) e marque como `<EmbeddedResource>` — o csproj já tem o padrão
`Migrations\*.sql`. Note que a substituição de variáveis do DbUp está
desligada (`WithVariablesDisabled()`) para não conflitar com strings que
contenham `$` (ex.: hashes BCrypt).

Cada `Postgres*Repository` usa Dapper com `IDbConnectionFactory` (uma
conexão por operação, sem pooling manual — o próprio Npgsql já cuida).

## 8. Docker

O [`docker-compose.yml`](docker-compose.yml) sobe apenas a API (o Postgres
é gerenciado no Render, então não é replicado localmente):

```powershell
$env:POSTGRES_CONNECTION_STRING = "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"
$env:JWT_SIGNING_KEY = "min-32-chars-please-change-me"
docker compose up --build
```
