# Marketplace Backend

API REST do MVP de Marketplace (modelo Mercado Livre, fluxo financeiro manual).
Construída em **.NET 9** com arquitetura **DDD em camadas** espelhando o esqueleto
do projeto LolOxiBot (`01-Api / 02-Domain / 03-Infra`).

> Aviso importante: nesta primeira iteração **não há autenticação real**
> (sem JWT, sem hash de senha, sem Serilog). Esse cross-cutting foi adiado para
> um próximo ciclo — antes de qualquer deploy público, adicionar JWT + BCrypt.

---

## 1. Stack

| Camada | Tecnologia |
|---|---|
| Runtime | .NET 9 (`net9.0`) |
| HTTP | ASP.NET Core Controllers + Swashbuckle |
| Persistência | **InMemoryStore (Singleton)** hoje · Dapper + Npgsql preparados para Postgres |
| Validação | FluentValidation |
| Logging | `ILogger<T>` built-in |
| Auth | "Fake token" (dicionário token→userId em memória) |

## 2. Estrutura de pastas

```
marketplace-backend/
├── Marketplace.sln
├── 01-Api/Marketplace.Api/              # Host ASP.NET Core (Controllers, Middlewares, Program.cs)
├── 02-Domain/
│   ├── Marketplace.Domain/              # Models, Enums, Interfaces (Repository + Service), Settings
│   └── Marketplace.Application/         # UseCases, DTOs, Validators, Exceptions
└── 03-Infra/
    ├── Marketplace.Setup/               # Composition root (IServiceCollectionExtensions.IocConfiguration)
    ├── Marketplace.Repository/          # Data/ (IDbConnectionFactory + Npgsql) · InMemory/ (atual) · Postgres/ (placeholder)
    └── Marketplace.Infrastructure/      # Auth/CurrentUserResolver · Shipping/MockShippingCalculator · Storage/LocalFileStorageService
```

## 3. Como rodar

```powershell
cd marketplace-backend
dotnet restore
dotnet run --project 01-Api/Marketplace.Api --launch-profile http
```

- API: `http://localhost:5066`
- Swagger UI: `http://localhost:5066/swagger`
- CORS já habilitado para `http://localhost:5173` (Vite dev) e `http://localhost:4173` (Vite preview).

### Integrando com o frontend

O frontend ([marketplace-frontend](../marketplace-frontend)) chama `baseURL: '/api'`
em [`src/lib/axios.ts`](../marketplace-frontend/src/lib/axios.ts). Duas opções:

1. **Vite proxy (recomendado)** — adicionar no `vite.config.ts` do frontend:
   ```ts
   server: {
     proxy: { '/api': 'http://localhost:5066' }
   }
   ```
2. **CORS direto** — não precisa de proxy; o backend já libera as origens dev.

Antes de subir o backend lembre-se de **desativar o MSW** no frontend
(`marketplace-frontend/src/main.tsx` ou onde está o `worker.start()`),
caso contrário o service worker intercepta as chamadas antes de chegarem na API.

## 4. Credenciais de teste

Os 3 usuários abaixo (senha `123456`) já são seedados na inicialização e
mantêm os **tokens fixos** prontos para uso direto (Swagger, Postman, `.http`):

| Role | E-mail | Senha | Token fake fixo |
|---|---|---|---|
| Buyer | `comprador@teste.com` | `123456` | `token-buyer` |
| Seller | `vendedor@teste.com` | `123456` | `token-seller` |
| Admin | `admin@teste.com` | `123456` | `token-admin` |

Para usar no Swagger, clique em "Authorize" e cole o token (sem o prefixo `Bearer`).
Em Postman/curl: `Authorization: Bearer token-admin`.

## 5. Endpoints (27 rotas, todas sob `/api`)

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| POST | `/api/auth/login` | público | `{email,password}` → `{user,token}` |
| POST | `/api/auth/register` | público | Cria conta buyer (201) |
| GET  | `/api/auth/me` | Bearer | User atual |
| GET  | `/api/products` | público | `?q=&categoryId=` filtros opcionais |
| GET  | `/api/products/:id` | público | 404 se não encontrar |
| GET  | `/api/categories` | público | Árvore plana |
| POST | `/api/orders` | Bearer | `{productId,quantity,address,shippingCost}` → `{order, pixKey}` (201) |
| POST | `/api/orders/:id/receipt` | Bearer (dono) | `multipart/form-data` campo `receipt` |
| GET  | `/api/orders/me` | Bearer | Histórico do buyer |
| POST | `/api/shipping/quote` | público | `{cepOrigem,cepDestino,peso}` (mock) |
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

### Contratos críticos preservados (paridade total com o mock MSW do frontend)

- `OrderStatus` serializado em **PT-BR com acentos literais**: `"Aguardando Comprovante"`, `"Em Análise"`, `"Pago"`, `"Enviado"`, `"Entregue"`.
- `UserRole` em minúsculas: `"buyer"`, `"seller"`, `"admin"`.
- Preços e pesos como `decimal` em reais (sem centavos, sem string).
- Datas em ISO-8601 UTC.
- Erros sempre `{ "message": "..." }` em português, com 400 / 401 / 403 / 404.
- JSON em **camelCase** com campos `null` explícitos (paridade com o shape do mock).

## 6. Regras de negócio relevantes

- **Aprovar pedido (`POST /api/admin/orders/:id/approve`)** debita o estoque do produto,
  muda o pedido para `Pago` e cria um `Repasse` com `commission = productPrice * CommissionRate`
  (default 10%, configurável em `appsettings.json::PlatformSettings.CommissionRate`) e
  `netAmount = productPrice + shippingCost - commission`.
- **`GET /api/seller/sales`** filtra somente status `Pago`, `Enviado` ou `Entregue`.
- **`GET /api/admin/orders`** retorna somente status `Em Análise`.
- **Criar pedido (`POST /api/orders`)** devolve `{ order, pixKey }`, onde `pixKey`
  vem de `PlatformSettings.PixKey` (default `marketplace@pix.com.br`).

## 7. Migração futura para PostgreSQL

A camada de persistência já está abstraída por interfaces no Domain
(`IUserRepository`, `IProductRepository`, ...). Hoje rodam implementações
`InMemory*` em [`Marketplace.Repository/InMemory/`](03-Infra/Marketplace.Repository/InMemory).

Para trocar quando o banco estiver pronto:

1. Preencher `ConnectionStrings:Postgress` em `appsettings.json`.
2. Implementar repositórios Dapper em [`03-Infra/Marketplace.Repository/Postgres/`](03-Infra/Marketplace.Repository/Postgres)
   usando o `IDbConnectionFactory` já registrado.
3. Em [`Marketplace.Setup/IServiceCollectionExtensions.cs`](03-Infra/Marketplace.Setup/IServiceCollectionExtensions.cs)
   trocar a chamada `RegisterRepositoriesInMemory()` por `RegisterRepositoriesPostgres()`.

UseCases, Controllers, DTOs e validators **não mudam**.

## 8. Próximos passos sugeridos

- **Segurança**: substituir o fake token por JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`) e hashar senha com `BCrypt.Net-Next`.
- **Persistência**: implementar repositórios Postgres com Dapper + scripts SQL versionados (DbUp ou FluentMigrator).
- **Comprovantes**: trocar `LocalFileStorageService` por integração com AWS S3.
- **Frete**: trocar `MockShippingCalculator` por integração real (Correios / Melhor Envio).
- **Logging estruturado**: adicionar Serilog com sink Console/File/Seq.
- **Testes**: criar projeto `Marketplace.UnitTests` com xUnit + FluentAssertions cobrindo UseCases.
