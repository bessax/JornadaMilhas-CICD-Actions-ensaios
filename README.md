# ✈️ JornadaMilhas API
 
API REST em **.NET 8** para gerenciamento de **ofertas de viagem** e **rotas**, com autenticação **JWT**, banco **SQL Server** e uma suíte de **testes de integração com Testcontainers**.
 
O projeto aplica conceitos de **DDD** (entidades, *Value Objects* e validação no domínio) e foi construído com foco em **qualidade e testes de software**.
 
---
 
## ✨ Funcionalidades
 
- CRUD completo de **Ofertas de Viagem** (rota, período e preço).
- CRUD completo de **Rotas** (origem e destino).
- **Paginação** de resultados e consulta do **último registro** cadastrado.
- **Autenticação JWT** (login com ASP.NET Core Identity) e endpoints protegidos com `RequireAuthorization`.
- **Validação no domínio**: rota com origem/destino obrigatórios, período com data de ida menor ou igual à de volta e preço maior que zero.
- **Migrations aplicadas automaticamente** na inicialização, com criação do banco caso não exista.
- **Seed automático**: usuário padrão e duas ofertas de exemplo.
- Documentação com **Swagger** (com suporte a Bearer Token).
- Resiliência de conexão com o banco (`EnableRetryOnFailure`).
- 
## 🏗️ Estrutura da solução
 
```
JornadaMilhas-API.sln
├── src/
│   ├── JornadaMilhas.API        # Minimal API, endpoints, DTOs, conversores e geração de token
│   ├── JornadaMilhas.Dominio    # Entidades (OfertaViagem, Rota), Value Object (Periodo) e validação
│   └── JornadaMilhas.Dados      # DbContext (Identity), EntityDAL<T> genérico e Migrations
└── test/
    ├── JornadaMilhas.Unity.Test               # Testes unitários do domínio
    └── JornadaMilhas.Integration.Test.API     # Testes de integração (WebApplicationFactory + Testcontainers)
```
 
## 🧰 Tecnologias
 
| Área | Tecnologias |
|---|---|
| Plataforma | C#, .NET 8 |
| API | ASP.NET Core Minimal APIs, Swagger (Swashbuckle), CORS |
| Autenticação | ASP.NET Core Identity, JWT Bearer |
| Dados | Entity Framework Core 8, SQL Server, Lazy Loading Proxies |
| Testes | xUnit, `Microsoft.AspNetCore.Mvc.Testing`, Testcontainers (SQL Server 2022), Coverlet |
 
## ✅ Pré-requisitos
 
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (para o SQL Server local e para os testes de integração)
## 🚀 Como executar
 
### 1. Clone o repositório
 
```bash
git clone https://github.com/<seu-usuario>/rec_donetarch3_jornadamilhas.git
cd rec_donetarch3_jornadamilhas
```
 
### 2. Suba um SQL Server com Docker
 
A string de conexão padrão (`src/JornadaMilhas.API/appsettings.json`) aponta para `localhost,11433`:
 
```bash
docker run -d --name sqlserver-jornadamilhas \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Alura#2024" \
  -p 11433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```
 
Se preferir outra instância/credenciais, altere `ConnectionString:DefaultConnection` no `appsettings.json`.
 
### 3. Execute a API
 
```bash
dotnet run --project src/JornadaMilhas.API --launch-profile https
```
 
Na primeira execução a API **cria o banco, aplica as migrations e insere os dados iniciais**.
 
- Swagger: `https://localhost:7280/swagger`
- HTTP: `http://localhost:5292/swagger`
## 🔐 Autenticação
 
Um usuário padrão é criado automaticamente na primeira execução:
 
| E-mail | Senha |
|---|---|
| `tester@email.com` | `Senha123@` |
 
Para consumir os endpoints protegidos:
 
1. Faça `POST /auth-login` com o corpo abaixo:
```json
{ "email": "tester@email.com", "password": "Senha123@" }
```
2. Copie o token retornado.
3. No Swagger, clique em **Authorize** e informe `Bearer <token>`.
   Em outras ferramentas, envie o cabeçalho `Authorization: Bearer <token>`.
## 📡 Endpoints
 
| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `POST` | `/auth-login` | Realiza o login e retorna o JWT | ❌ |
| `GET` | `/ofertas-viagem` | Lista todas as ofertas | ✅ |
| `GET` | `/ofertas-viagem/{id}` | Obtém oferta por id | ✅ |
| `GET` | `/ofertas-viagem/{pagina}/{tamanhoPorPagina}` | Lista paginada | ✅ |
| `GET` | `/ofertas-viagem/ultimo-registro` | Última oferta cadastrada | ✅ |
| `POST` | `/ofertas-viagem` | Cadastra oferta | ✅ |
| `PUT` | `/ofertas-viagem` | Atualiza período e preço | ✅ |
| `DELETE` | `/ofertas-viagem/{id}` | Remove oferta | ✅ |
| `GET` | `/rota-viagem` | Lista todas as rotas | ✅ |
| `GET` | `/rota-viagem/{id}` | Obtém rota por id | ✅ |
| `GET` | `/rota-viagem/{pagina}/{tamanhoPorPagina}` | Lista paginada | ✅ |
| `GET` | `/rota-viagem/ultimo-registro` | Última rota cadastrada | ✅ |
| `POST` | `/rota-viagem` | Cadastra rota | ✅ |
| `PUT` | `/rota-viagem` | Atualiza origem e destino | ✅ |
| `DELETE` | `/rota-viagem/{id}` | Remove rota | ✅ |
 
Exemplo de corpo para `POST /ofertas-viagem`:
 
```json
{
  "rota": { "origem": "Vitória-ES", "destino": "São Paulo-SP" },
  "periodo": { "dataInicial": "2024-01-01", "dataFinal": "2024-01-05" },
  "preco": 250.0
}
```
 
## 🧪 Testes
 
```bash
# Todos os testes
dotnet test
 
# Apenas os testes unitários
dotnet test test/JornadaMilhas.Unity.Test
 
# Apenas os de integração (requer Docker em execução)
dotnet test test/JornadaMilhas.Integration.Test.API
```
 
Os **testes de integração** sobem a API em memória com `WebApplicationFactory` e criam um **SQL Server 2022 efêmero em container** via Testcontainers, de modo que não afetam o seu banco local. Eles cobrem:
 
- login com sucesso e com falha;
- cadastro, consulta, paginação, atualização e exclusão de ofertas e rotas;
- cenários inválidos: rota nula, token inválido e rota de endpoint vazia.
## ⚠️ Segurança
 
Este é um projeto de estudo, e o `appsettings.json` contém valores de exemplo (senha do SQL Server e chave do JWT). Em qualquer ambiente real:
 
- não versione segredos: use **User Secrets**, variáveis de ambiente ou um cofre (Azure Key Vault, por exemplo);
- gere uma nova chave `JWTKey:key` forte;
- restrinja a política de **CORS** (hoje permite qualquer origem);
- remova o usuário padrão `tester@email.com`.
## 🗺️ Próximos passos
 
- [ ] Reativar o endpoint de registro de usuários (`/auth-registrar`, hoje comentado).
- [ ] Reativar os testes de listagem e consulta por id de ofertas (hoje com `[Fact]` comentado).
- [ ] Ampliar os testes unitários do domínio (`Rota`, `Periodo` e `OfertaViagem`).
- [ ] Retornar `400 Bad Request` para dados inválidos (hoje retorna `500` via `Results.Problem`).
- [ ] Adicionar `Dockerfile` e `docker-compose` (API + SQL Server).
- [ ] Configurar pipeline de CI para build e testes.
## 🤝 Contribuindo
 
Sugestões e *pull requests* são bem-vindos! Abra uma *issue* para conversarmos sobre melhorias.
 
## 📄 Licença
 
Defina aqui a licença do projeto (por exemplo, [MIT](https://opensource.org/licenses/MIT)).
 
---
 
Feito com 💙 durante os estudos de **.NET, DDD e testes de software**.
 
