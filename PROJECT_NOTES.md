# Shopping Cart - Notas do Projeto

> Documentação técnica e resumo do estado atual do projeto
>
> **Última atualização:** 17 de Dezembro de 2025

---

## 📋 Resumo do Projeto

Sistema de carrinho de compras desenvolvido em **ASP.NET Core 9.0** utilizando **Clean Architecture** com 4 camadas:

- **Domain**: Entidades e interfaces de negócio (sem dependências externas)
- **Application**: Lógica de negócio, DTOs e interfaces de serviços
- **Infrastructure**: Acesso a dados e integrações com serviços externos
- **Api**: Controladores REST e configuração de injeção de dependências

---

## 🛠️ Stack Tecnológica

### Backend
- **Framework**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core
- **Autenticação**: JWT (JSON Web Tokens)
- **Validação**: FluentValidation

### Infraestrutura
- **Banco de Dados**: PostgreSQL 15
- **Cache**: Redis 7.2
- **Message Broker**: Kafka 7.5.0 (com Zookeeper)
- **Fila de Mensagens**: RabbitMQ 3 (configurado, não utilizado ainda)

### Containerização
- **Docker Compose** para todos os serviços de infraestrutura

---

## 🗂️ Entidades Implementadas

### 1. UserEntity
```csharp
- Id: string (GUID)
- Email: string (único, obrigatório)
- PasswordHash: string (BCrypt)
- Name: string
- CreatedAt: DateTime
- UpdatedAt: DateTime?
- IsActive: bool
```

**Localização:** `Domain/Entities/UserEntity.cs`

**Funcionalidades:**
- Email único garantido a nível de banco de dados
- Soft delete (IsActive flag)
- Senha criptografada com BCrypt

---

### 2. ProductEntity
```csharp
- Id: string (GUID)
- Name: string (max 200 chars, obrigatório)
- Description: string (max 1000 chars)
- Value: decimal(18,2)
- ImageUrl: string?
- IsActive: bool
- CreatedAt: DateTime
- UpdatedAt: DateTime?
```

**Localização:** `Domain/Entities/ProductEntity.cs`

**Funcionalidades:**
- CRUD completo
- Suporte a bulk insert
- Atualização parcial (PATCH)

---

### 3. CategoryEntity
```csharp
- Id: string (GUID)
- Name: string (max 200 chars, obrigatório)
- Description: string? (max 1000 chars)
- IsActive: bool
- CreatedAt: DateTime
- UpdatedAt: DateTime?
```

**Localização:** `Domain/Entities/CategoryEntity.cs`

**Funcionalidades:**
- CRUD completo
- Atualização parcial (PATCH)

---

### 4. Cart & CartItem
```csharp
// Cart
- UserEmail: string
- Items: List<CartItem>

// CartItem
- ProductId: string
- Quantity: int
```

**Localização:** `Domain/Entities/Cart.cs`, `Domain/Entities/CartItem.cs`

**Funcionalidades:**
- Armazenamento no Redis para alta performance
- Processamento assíncrono via eventos Kafka

---

## 🚀 Funcionalidades Implementadas

### ✅ Autenticação e Autorização

**Endpoints:** `/api/auth`

- `POST /api/auth/register` - Registro de novos usuários
  - Validação de email único
  - Hash de senha com BCrypt
  - Validação com FluentValidation

- `POST /api/auth/login` - Login de usuários
  - Validação de credenciais
  - Geração de JWT token (8h de validade)
  - Geração de refresh token (24h de validade)

**Configuração JWT:**
- Algoritmo: HMAC-SHA256
- Issuer: `shopping_cart`
- Secret configurável via appsettings.json

---

### ✅ Gestão de Produtos

**Endpoints:** `/api/products`

- `GET /api/products` - Listar todos os produtos
- `GET /api/products/{id}` - Buscar produto por ID
- `POST /api/products` - Criar produtos em bulk (aceita lista)
- `PUT /api/products/{id}` - Atualização completa
- `PATCH /api/products/{id}` - Atualização parcial
- `DELETE /api/products/{id}` - Deletar produto

**Características:**
- Suporte a bulk insert para performance
- Operações PATCH para updates parciais
- Logging de operações
- Retorno nullable para melhor null safety

---

### ✅ Gestão de Categorias

**Endpoints:** `/api/categories`

- `GET /api/categories` - Listar todas as categorias
- `GET /api/categories/{id}` - Buscar categoria por ID
- `POST /api/categories` - Criar categoria
- `PUT /api/categories/{id}` - Atualização completa
- `PATCH /api/categories/{id}` - Atualização parcial
- `DELETE /api/categories/{id}` - Deletar categoria

**Características:**
- CRUD completo
- Suporte a campos nullable em updates
- Validação de dados

---

### ✅ Carrinho de Compras (Event-Driven)

**Endpoints:** `/api/cart`

- `GET /api/cart?userEmail={email}` - Obter carrinho do usuário
- `POST /api/cart/items?userEmail={email}` - Adicionar item ao carrinho
  - Body: `{ ProductId, Quantity }`
- `DELETE /api/cart/items?userEmail={email}` - Remover item do carrinho
  - Body: `{ ProductId }`

**Arquitetura:**
- **Eventos Kafka**: Operações publicam eventos no tópico `worker-cart-events`
- **Background Consumer**: `CartEventsConsumer` processa eventos assincronamente
- **Armazenamento Redis**: Cache de alta performance para carrinhos ativos
- **Tipos de Eventos**:
  - `CartItemAddedEvent`: Adiciona ou atualiza quantidade
  - `CartItemRemovedEvent`: Remove item do carrinho

**Vantagens:**
- Desacoplamento entre API e processamento
- Alta performance via Redis
- Escalabilidade horizontal

---

## 📁 Estrutura de Pastas

```
shopping-cart/
├── Api/                                # Camada de apresentação
│   ├── Controllers/                    # Controladores REST
│   │   ├── AuthController.cs
│   │   ├── ProductController.cs
│   │   ├── CategoryController.cs
│   │   ├── CartController.cs
│   │   └── UserController.cs
│   ├── Builders/                       # Configurações de injeção de dependências
│   │   ├── AuthBuilder.cs
│   │   ├── KafkaBuilder.cs
│   │   ├── RedisBuilder.cs
│   │   └── ServicesBuilder.cs
│   └── appsettings.json                # Configurações da aplicação
│
├── Application/                        # Camada de aplicação
│   ├── Dtos/                           # Data Transfer Objects
│   │   ├── Auth/
│   │   ├── Cart/
│   │   ├── Categories/
│   │   ├── Events/
│   │   ├── Products/
│   │   └── Users/
│   ├── Interfaces/
│   │   ├── Repositories/               # Interfaces de repositórios
│   │   └── Services/                   # Interfaces de serviços
│   ├── Services/                       # Implementação de serviços de negócio
│   │   ├── AuthService.cs
│   │   ├── CartService.cs
│   │   ├── CategoryService.cs
│   │   └── ProductService.cs
│   └── Validators/                     # FluentValidation validators
│
├── Domain/                             # Camada de domínio
│   ├── Entities/                       # Entidades de negócio
│   │   ├── Cart.cs
│   │   ├── CartItem.cs
│   │   ├── CategoryEntity.cs
│   │   ├── ProductEntity.cs
│   │   └── UserEntity.cs
│   └── Interfaces/
│       └── Repositories/               # Interfaces de repositórios do domínio
│
├── Infrastructure/                     # Camada de infraestrutura
│   ├── Data/
│   │   ├── Configurations/             # Fluent API configurations
│   │   ├── Migrations/                 # EF Core migrations
│   │   └── AppDbContext.cs
│   ├── Repositories/                   # Implementação de repositórios
│   │   ├── CategoryRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── RedisCartRepository.cs
│   │   └── UserRepository.cs
│   ├── Services/                       # Serviços de infraestrutura
│   │   ├── KafkaEventPublisher.cs
│   │   ├── PasswordHasher.cs
│   │   └── TokenService.cs
│   └── BackgroundServices/
│       └── CartEventsConsumer.cs       # Consumidor Kafka
│
└── docker-compose.yaml                 # Orquestração de containers
```

---

## 🔧 Configurações

### Database (PostgreSQL)
```json
"DefaultConnection": "Host=localhost;Port=5432;Database=shopping_cart;Username=postgres;Password=postgres"
```

### Redis
```json
"Redis": "localhost:6379"
```

### Kafka
```json
"Kafka": {
  "BootstrapServers": "localhost:9092"
}
```

**Tópico:** `worker-cart-events`
**Consumer Group:** `cart-events`

### JWT
```json
"JWT": {
  "Secret": "umhashdequalquercoisaparacolocaraquinessaplicacaodeteste",
  "Issuer": "shopping_cart"
}
```

---

## 📊 Migrations Aplicadas

1. **CreateUserTable** (2025-06-05)
   - Criação da tabela de usuários

2. **CreateProductsTable** (2025-06-12)
   - Criação da tabela de produtos

3. **UpdateEmailToBeUniqueInUsers** (2025-10-27)
   - Constraint de email único a nível de banco de dados

---

## 🎯 Padrões e Decisões Arquiteturais

### Clean Architecture
- Separação clara de responsabilidades
- Domain não depende de nenhuma camada externa
- Fluxo de dependências: Api → Application → Infrastructure → Domain

### Repository Pattern
- Abstração de acesso a dados
- Interfaces no Domain/Application, implementações no Infrastructure
- Facilita testes e manutenção

### Event-Driven Architecture (Carrinho)
- Desacoplamento via eventos Kafka
- Processamento assíncrono com background service
- Resiliência e escalabilidade

### Dependency Injection
- Scoped: Repositórios, Serviços, Logger
- Singleton: Redis, Kafka Producer/Consumer

### DTO Pattern
- CreateDto: Para criação de entidades
- UpdateDto: Para atualização (nullable properties)
- ResultDto: Para retorno com timestamps de auditoria

### Fluent Validation
- Validação centralizada e reutilizável
- Mensagens de erro customizadas
- Auto-wired via assembly scanning

### Soft Delete
- Flag `IsActive` em entidades
- Métodos `DeactivateAsync()` e `ReactivateAsync()` em repositórios
- Preservação de dados históricos

### Null Safety
- Nullable reference types habilitado
- Repositórios retornam `Task<T?>`
- Tratamento adequado de nulls nos serviços

---

## 📈 Próximos Passos Sugeridos

### 🔥 Alta Prioridade

#### 1. Relacionamento Produto-Categoria
**Motivo:** Produtos e categorias existem, mas não estão relacionados

**Implementação:**
- [ ] Adicionar `CategoryId` em `ProductEntity`
- [ ] Criar migration para adicionar FK
- [ ] Atualizar DTOs de produto para incluir categoria
- [ ] Atualizar serviços e endpoints
- [ ] Adicionar filtro de produtos por categoria

**Estimativa de impacto:** Alto - Funcionalidade core de e-commerce

---

#### 2. Implementação de UserController
**Motivo:** Controller existe mas está vazio

**Implementação:**
- [ ] Criar UserService completo
- [ ] Endpoints para gestão de perfil:
  - GET /api/users/me (perfil do usuário autenticado)
  - PUT /api/users/me (atualizar perfil)
  - PATCH /api/users/me/password (trocar senha)
  - DELETE /api/users/me (desativar conta)
- [ ] Adicionar autorização JWT aos endpoints
- [ ] Admin endpoints (listar usuários, ativar/desativar)

**Estimativa de impacto:** Médio - Gestão de usuários

---

#### 3. Sistema de Pedidos (Orders)
**Motivo:** Próximo passo natural após carrinho

**Implementação:**
- [ ] Criar `OrderEntity` e `OrderItemEntity`
- [ ] Estados do pedido (Pending, Processing, Shipped, Delivered, Cancelled)
- [ ] OrderService para checkout do carrinho
- [ ] Endpoints:
  - POST /api/orders (criar pedido a partir do carrinho)
  - GET /api/orders (listar pedidos do usuário)
  - GET /api/orders/{id} (detalhes do pedido)
  - PATCH /api/orders/{id}/status (atualizar status - admin)
- [ ] Event-driven para processamento de pedidos
- [ ] Integração com sistema de pagamento (futuro)

**Estimativa de impacto:** Crítico - Core business

---

### ⚡ Média Prioridade

#### 4. Sistema de Estoque (Inventory)
**Motivo:** Controlar disponibilidade de produtos

**Implementação:**
- [ ] Adicionar `StockQuantity` em ProductEntity
- [ ] Criar `InventoryService` para controle de estoque
- [ ] Validação de estoque ao adicionar ao carrinho
- [ ] Reserva de estoque ao criar pedido
- [ ] Eventos de baixa/reposição de estoque
- [ ] Endpoints admin para gestão de estoque

**Estimativa de impacto:** Alto - Previne overselling

---

#### 5. Refresh Token Endpoint
**Motivo:** JWT expira em 8h, precisa de renovação sem re-login

**Implementação:**
- [ ] Criar tabela `RefreshTokens` para armazenar tokens
- [ ] Endpoint POST /api/auth/refresh
- [ ] Validação de refresh token
- [ ] Geração de novo access token
- [ ] Revogação de refresh tokens antigos
- [ ] Endpoint de logout (revogar tokens)

**Estimativa de impacto:** Alto - UX e segurança

---

#### 6. Imagens de Produtos
**Motivo:** `ImageUrl` existe mas não há upload

**Implementação:**
- [ ] Endpoint para upload de imagens
- [ ] Armazenamento local ou cloud (S3, Azure Blob)
- [ ] Validação de tipo e tamanho de arquivo
- [ ] Geração de thumbnails
- [ ] Múltiplas imagens por produto
- [ ] CDN para servir imagens (futuro)

**Estimativa de impacto:** Médio - UX

---

#### 7. Sistema de Avaliações (Reviews)
**Motivo:** Aumentar confiança e engajamento

**Implementação:**
- [ ] Criar `ReviewEntity` (ProductId, UserId, Rating, Comment)
- [ ] Apenas usuários com pedido entregue podem avaliar
- [ ] Média de rating no produto
- [ ] Endpoints CRUD para reviews
- [ ] Moderação de reviews (admin)

**Estimativa de impacto:** Médio - Social proof

---

### 🔍 Baixa Prioridade / Melhorias

#### 8. Testes Automatizados
**Motivo:** Garantir qualidade e facilitar refatorações

**Implementação:**
- [ ] Testes unitários para services (xUnit)
- [ ] Testes de integração para repositories
- [ ] Testes de API (endpoints)
- [ ] Mocks com Moq ou NSubstitute
- [ ] Coverage mínimo de 80%

**Estimativa de impacto:** Alto - Qualidade e manutenção

---

#### 9. Observabilidade
**Motivo:** Monitoramento e troubleshooting em produção

**Implementação:**
- [ ] Structured logging com Serilog
- [ ] Correlação de logs com correlation IDs
- [ ] Métricas com Prometheus
- [ ] Tracing distribuído (OpenTelemetry)
- [ ] Health checks endpoints
- [ ] Dashboard com Grafana

**Estimativa de impacto:** Médio - Operação

---

#### 10. Integração com RabbitMQ
**Motivo:** RabbitMQ está configurado mas não utilizado

**Implementação:**
- [ ] Definir casos de uso (emails, notificações)
- [ ] Criar publishers e consumers
- [ ] Padrão de retry e dead letter queue
- [ ] Comparar com Kafka para escolher uso adequado

**Estimativa de impacto:** Baixo - Redundante com Kafka

---

#### 11. Documentação OpenAPI Aprimorada
**Motivo:** Facilitar integração e uso da API

**Implementação:**
- [ ] Adicionar XML comments nos controllers
- [ ] Exemplos de request/response
- [ ] Descrições detalhadas de endpoints
- [ ] Documentar códigos de erro
- [ ] Versionamento de API

**Estimativa de impacto:** Médio - DX

---

#### 12. Rate Limiting
**Motivo:** Proteção contra abuso

**Implementação:**
- [ ] Middleware de rate limiting
- [ ] Limites por endpoint e por usuário
- [ ] Headers de rate limit na resposta
- [ ] Redis para armazenar contadores

**Estimativa de impacto:** Médio - Segurança

---

#### 13. Internacionalização (i18n)
**Motivo:** Suporte a múltiplos idiomas

**Implementação:**
- [ ] Resource files para mensagens
- [ ] Localização de validações
- [ ] Produtos com nomes/descrições multi-idioma
- [ ] Header Accept-Language

**Estimativa de impacto:** Baixo - Depende do mercado-alvo

---

## 📝 Status Atual do Git

### Arquivos Modificados
- `Api/Controllers/CategoryController.cs`
- `Api/appsettings.json`

### Arquivos Staged (Prontos para Commit)
- `Application/Dtos/Categories/CategoryCreateDto.cs`
- `Application/Dtos/Categories/CategoryUpdateDto.cs`
- `Application/Interfaces/Services/ICategoryService.cs`
- `Application/Services/CategoryService.cs`
- `Domain/Entities/CategoryEntity.cs`
- `Infrastructure/Data/AppDbContext.cs`

### Arquivos Não Rastreados
- `Api/.claude/`
- `Application/Dtos/Categories/CategoryResultDto.cs`
- `Domain/Interfaces/Repositories/ICategoryRepository.cs`
- `Infrastructure/Data/Configurations/CategoryEntityConfiguration.cs`
- `Infrastructure/Repositories/CategoryRepository.cs`

**Ação Recomendada:** Commit das funcionalidades de Category

---

## 🎓 Commits Recentes

1. **Implement bulk insert functionality for products and update email uniqueness in users**
2. **Refactor product service and repository to support nullable return types and add partial update functionality**
3. **Add authentication, Kafka, and Redis configuration builders**
4. **Add Docker Compose configuration**

---

## 🚀 Como Executar

### Pré-requisitos
```bash
# Subir infraestrutura
docker-compose up -d

# Aplicar migrations
dotnet ef database update --project Infrastructure --startup-project Api
```

### Executar Aplicação
```bash
cd Api
dotnet run
```

### Acessar Swagger
```
http://localhost:<porta>/swagger
```

---

## 📞 Endpoints de Health Check

**Recomendação:** Adicionar endpoints de health check

```csharp
// Sugestão
GET /health
GET /health/ready
GET /health/live
```

---

## 💡 Observações Finais

Este projeto demonstra uma implementação sólida de Clean Architecture com:

- ✅ Separação clara de responsabilidades
- ✅ Event-driven design para operações assíncronas
- ✅ Cache com Redis para performance
- ✅ Message broker com Kafka para escalabilidade
- ✅ Autenticação JWT robusta
- ✅ Validação com FluentValidation
- ✅ Soft delete para auditoria
- ✅ Nullable safety para prevenir NullReferenceException

**Pontos Fortes:**
- Arquitetura escalável e testável
- Uso adequado de padrões (Repository, DI, DTO)
- Infraestrutura moderna (Kafka, Redis, PostgreSQL)

**Oportunidades de Melhoria:**
- Implementar testes automatizados
- Adicionar observabilidade (logging estruturado, métricas)
- Completar funcionalidades de negócio (Orders, Inventory)
- Melhorar documentação da API

---

**Última revisão:** 17/12/2025
**Versão do .NET:** 9.0
**Autor:** Sistema gerado automaticamente
