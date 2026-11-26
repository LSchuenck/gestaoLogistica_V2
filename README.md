# Gestão Logística — API ASP.NET Core + Front-end Vue.js

Trabalho integrado das disciplinas **Desenvolvimento Web III** (front-end em Vue.js) e **Engenharia de Software** (API em ASP.NET Core com Clean Architecture).

## Integrantes

- Lucas Faria Schuenck de Souza
- Alexandre João Souza de Araújo
- Cauan Etienne Andrade e Silva

## Descrição do sistema

Evolução do sistema **Gestão Logística**, desenvolvido anteriormente em PHP + MySQL. A lógica de negócio e o acesso ao banco saíram das páginas PHP e passaram para uma **API REST em C#**. O front-end foi refeito em **Vue.js** e consome essa API.

Módulos:

| Módulo | O que faz | Principais regras de negócio |
|---|---|---|
| Usuários | Cadastro, edição, ativação e exclusão (somente ADMIN) | E-mail único; perfis ADMIN, GERENTE e OPERADOR; o administrador master (id 1) não pode ser excluído, desativado nem perder o perfil ADMIN; ninguém exclui ou desativa a própria conta; não exclui usuário com movimentações de estoque; usuário inativo não faz login; o cadastro gera uma senha temporária, enviada por e-mail, que precisa ser trocada no primeiro acesso; senha com no mínimo 6 caracteres guardada com hash BCrypt |
| Transportadoras | CRUD + ativar/desativar | CNPJ único; não pode ser excluída se tiver motoristas ou veículos |
| Motoristas | CRUD | Só pode ser ligado a uma transportadora **ativa**; CPF e CNH únicos; indica CNH vencida |
| Veículos | CRUD + troca de status | Placa única; transportadora ativa; status só DISPONIVEL, EM_VIAGEM ou MANUTENCAO; não exclui veículo em viagem |
| Clientes | CRUD com endereço | — |
| Armazéns | CRUD com endereço | Não pode ser excluído se tiver produtos com saldo ou movimentações |
| Produtos | CRUD | Indica produto vencido e estoque crítico (menos de 10 unidades no total); não exclui produto com saldo ou movimentações |
| Estoque | Entrada/saída por armazém, saldos e histórico | Quantidade maior que zero; saída não pode ser maior que o saldo do armazém; saldo e histórico são gravados juntos, com o produto travado no banco (duas saídas ao mesmo tempo não furam o saldo); registra o usuário que fez a movimentação |
| Viagens | Iniciar, avançar, concluir e cancelar o transporte de um produto, de um armazém até um cliente | O produto sai do estoque da origem ao iniciar e volta se a viagem for cancelada; não passa do saldo nem da capacidade do veículo; veículo disponível; motorista com CNH válida, da mesma transportadora e sem outra viagem; só conclui depois de avançar; faturamento = distância (km) x valor do frete por km, e viagem cancelada não fatura; o veículo fica EM_VIAGEM e volta a DISPONIVEL no fim; não exclui veículo, motorista, cliente ou armazém com viagens |

Permissões por perfil (as mesmas do sistema original):

| Perfil | Acesso |
|---|---|
| ADMIN | Tudo |
| GERENTE | Tudo, menos Usuários |
| OPERADOR | Somente Estoque e Viagens |

## Tecnologias

**API:** C# / .NET 10, ASP.NET Core Web API, Entity Framework Core 9 + Pomelo (MySQL), JWT Bearer, BCrypt.Net-Next, IMemoryCache.

**Front-end:** Vue 3, Vite, Vue Router, Pinia, Vuetify, Axios.

**Banco de dados:** MySQL 8 (também funciona com o MariaDB do XAMPP).

**Implantação:** Docker e Docker Compose.

## Arquitetura

```
Aplicação Vue  ──HTTP/JSON + JWT──▶  API ASP.NET Core  ──EF Core──▶  MySQL
```

A API segue a **Clean Architecture**, dividida em 4 projetos:

```
api/
└── src/
    ├── GestaoLogistica.Domain          → Entidades, regras do domínio, constantes e interfaces dos repositórios
    ├── GestaoLogistica.Application     → Serviços (casos de uso), DTOs, ResultWrapper e interfaces de segurança
    ├── GestaoLogistica.Infrastructure  → EF Core (ContextoBanco), repositórios, JWT, BCrypt e dados iniciais
    └── GestaoLogistica.API             → Controllers, configuração (JWT, CORS), middleware de erros
```

Dependências entre as camadas (as setas apontam para quem é usado):

```
API ──▶ Application ──▶ Domain
 │                        ▲
 └──▶ Infrastructure ─────┘  (Infrastructure também implementa as interfaces da Application)
```

Decisões principais:

- **Domain não depende de ninguém.** As interfaces dos repositórios (`IRepositorioProduto` etc.) ficam no Domain e são implementadas na Infrastructure. Regras simples ficam nas próprias entidades (ex.: `Estoque.TemSaldoSuficiente`, `Motorista.CnhEstaVencida`, `Transportadora.AlternarStatus`).
- **Os controllers não têm lógica.** Eles só recebem a requisição, chamam o serviço e devolvem o `ResultWrapper`. As regras ficam nos serviços da Application (`ServicoTransportadora`, `ServicoEstoque`...).
- **Application não conhece BCrypt, JWT nem EF Core.** Ela usa as interfaces `ICriptografiaSenha` e `IGeradorToken`, implementadas na Infrastructure.
- **Movimentação de estoque com trava:** o `ServicoEstoque` confere o saldo e grava dentro de uma transação com `SELECT ... FOR UPDATE` no produto. Uma movimentação do mesmo produto espera a outra terminar. As viagens usam a mesma ideia (`RepositorioViagem`), para o estoque não ser devolvido duas vezes.
- **Repositório base genérico** (`RepositorioBase<T>`) com o CRUD comum, para não repetir código. Cada repositório específico só acrescenta suas consultas.
- **Injeção de dependência:** cada camada registra seus serviços (`AdicionarCamadaAplicacao` e `AdicionarCamadaInfraestrutura`).
- **Endereço** é um *owned type* do EF Core: as colunas ficam na tabela da transportadora, do cliente ou do armazém. No sistema PHP era uma tabela separada; assim o código ficou mais simples.
- **Banco criado automaticamente** com `EnsureCreated()` na primeira execução, já com os dados de exemplo do sistema original.

Estrutura do front-end (mesmo padrão usado nas aulas):

```
front/src/
├── plugin/axios.js     → instância do axios, envia o token e trata o 401
├── router/index.js     → rotas de cada página + proteção (login obrigatório, Usuários só para ADMIN)
├── stores/             → stores Pinia (auth, mensagem e uma por módulo)
├── views/              → uma página por rota
├── components/         → Lista<Modulo>.vue e Form<Modulo>.vue + componentes reutilizados
└── utils/              → formatação de datas/números e regras de validação
```

## Como executar

Há duas formas: com Docker (mais simples) ou sem Docker (para desenvolver). As duas usam as portas 5080 e 5173, então rode **uma de cada vez**.

## Executar com Docker

Pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/) aberto.

1. Na pasta do projeto, crie o arquivo de senhas e preencha:

```bash
copy .env.exemplo .env
```

2. Suba o sistema:

```bash
docker compose up -d --build
```

3. Acesse `http://localhost:5173`.

| Serviço | Imagem | Endereço |
|---|---|---|
| `banco` | `mysql:8.4` | só dentro da rede do Docker |
| `api` | `api/Dockerfile` (.NET 10) | `http://localhost:5080` |
| `front` | `front/Dockerfile` (Vue + Nginx) | `http://localhost:5173` |

- A API só inicia depois que o MySQL está pronto (`healthcheck` + `depends_on`).
- Os dados ficam no volume `dados_mysql` e continuam lá quando os containers são recriados.
- O banco do Docker é separado do MySQL instalado na máquina.

| Comando | O que faz |
|---|---|
| `docker compose ps` | Mostra os containers |
| `docker compose logs -f api` | Mostra o log da API (inclusive o `[CACHE]`) |
| `docker compose down` | Para tudo e mantém os dados |
| `docker compose down -v` | Para tudo e **apaga os dados** |

As senhas do banco são gravadas na primeira execução. Para trocá-las depois, é preciso recriar o banco com `docker compose down -v`.

## Executar sem Docker

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js 22.18+ ou 24.12+](https://nodejs.org)
- MySQL 8 (ou XAMPP com MySQL/MariaDB)

### 1. Configurar o banco e o JWT

Copie o arquivo de exemplo e preencha com seus dados:

```bash
cd api/src/GestaoLogistica.API
copy appsettings.Development.exemplo.json appsettings.Development.json
```

```json
{
  "ConnectionStrings": {
    "BancoGestaoLogistica": "Server=localhost;Port=3306;Database=gestao_logistica_api;User=root;Password=SUA_SENHA;"
  },
  "AdministradorInicial": {
    "Senha": "SENHA_DO_PRIMEIRO_ADMINISTRADOR"
  },
  "Email": {
    "UrlPowerAutomate": "ENDERECO_DO_FLUXO_DO_POWER_AUTOMATE"
  },
  "Jwt": {
    "Chave": "UMA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES"
  }
}
```

> O `appsettings.Development.json` está no `.gitignore`: senha do banco, chave do JWT, senha inicial do administrador e endereço do Power Automate **não vão para o repositório**. Também é possível passar tudo por variáveis de ambiente (`ConnectionStrings__BancoGestaoLogistica`, `Jwt__Chave`, `AdministradorInicial__Senha` e `Email__UrlPowerAutomate`).

Não é preciso criar o banco: na primeira execução a API cria o banco `gestao_logistica_api`, as tabelas e os dados de exemplo (transportadoras, motoristas, veículos, clientes, armazéns, produtos e estoque).

### 2. Executar a API

```bash
cd api
dotnet run --project src/GestaoLogistica.API
```

A API fica em `http://localhost:5080`.

### 3. Executar o front-end

```bash
cd front
npm install
npm run dev
```

Acesse `http://localhost:5173`. O endereço da API está no arquivo `front/.env` (`VITE_API_URL`).

### Usuário padrão

| E-mail | Senha | Perfil |
|---|---|---|
| admin@gestao.com | a que você definiu (`SENHA_ADMIN` no `.env` do Docker, ou `AdministradorInicial:Senha` sem Docker) | ADMIN |

Se o banco já existia em uma versão anterior, recrie-o ou execute o que faltar de `api/atualizar_banco.sql`.

Essa senha só é usada quando o banco é criado. Depois, altere pela tela de Usuários.

## Padrão de respostas (ResultWrapper)

Todas as respostas da API, de sucesso ou de erro, têm o mesmo formato (classe `ResultWrapper<T>` em `Application/Comum`):

```json
{
  "sucesso": true,
  "mensagem": "Produto encontrado.",
  "dados": { "id": 10, "descricao": "Luva Nitrílica Descartável cx/100", "quantidadeTotal": 92 },
  "erros": []
}
```

```json
{
  "sucesso": false,
  "mensagem": "Saldo insuficiente neste armazém. Disponível: 7 unidades.",
  "dados": null,
  "erros": ["Saldo insuficiente neste armazém. Disponível: 7 unidades."]
}
```

O código HTTP acompanha o resultado (200, 201, 400, 401, 403, 404, 500). Erros de validação dos DTOs, falta de token (401), falta de permissão (403), endereço inexistente (404), método não permitido (405), formato não suportado (415) e erros inesperados (500, no `TratamentoErrosMiddleware`) também são devolvidos nesse formato.

## Autenticação JWT

1. O usuário envia e-mail e senha para `POST /api/autenticacao/login` (único endpoint público).
2. A API busca o usuário e compara a senha com o hash **BCrypt** guardado no banco (a senha nunca é salva em texto puro).
3. Se estiver correto e o usuário estiver ativo, a API gera um token JWT assinado (HMAC-SHA256) com: id, nome, e-mail e perfil. O token vale 60 minutos (`Jwt:ExpiracaoMinutos`).
4. O front guarda o token no `localStorage` e o axios envia em todas as requisições no cabeçalho `Authorization: Bearer <token>`.
5. Em cada requisição a API valida assinatura, emissor, público e validade. Sem token válido → **401**. Os endpoints de `/api/usuarios` exigem perfil `ADMIN` (`[Authorize(Roles = "ADMIN")]`) → **403** para os demais perfis.
6. A API também confere, a cada requisição, se o usuário ainda existe, está ativo e tem o mesmo perfil do token (`TokenAindaEhValido`). Quem foi desativado perde o acesso na hora → **401**.
7. Se o token expirar, o front recebe 401, limpa o login e volta para a tela de login com o aviso “Sua sessão expirou”.

### Primeiro acesso

1. O ADMIN cadastra o usuário sem informar senha. A API gera uma senha temporária de 10 caracteres.
2. A senha é enviada por e-mail, por um fluxo do **Microsoft Power Automate**. Se o envio falhar, a senha aparece na tela para o ADMIN repassar.
3. No primeiro login, o token traz a informação `TrocarSenha`. O `TrocaSenhaMiddleware` bloqueia toda a API (403), menos `PUT /api/autenticacao/senha`.
4. Depois da troca, a API devolve um novo token e o acesso é liberado.

O endereço do fluxo tem uma assinatura secreta, por isso fica na configuração (`URL_POWER_AUTOMATE` no Docker, ou `Email:UrlPowerAutomate` sem Docker). Ele é opcional: vazio, a senha temporária aparece na tela.

## Estratégia de cache

**Onde:** listagem de transportadoras (`GET /api/transportadoras`), em `ServicoTransportadora`.

**Por quê:** essa lista é usada na tela de transportadoras e também nos formulários de motoristas e veículos (para escolher a transportadora), mas muda pouco.

**Como:** cache em memória (`IMemoryCache`) com **expiração absoluta de 5 minutos**.

| Situação | O que acontece | Como identificar |
|---|---|---|
| Dado buscado na fonte original | Não há nada no cache → consulta o banco e guarda o resultado | Log `[CACHE] Transportadoras buscadas no BANCO DE DADOS.` e mensagem da resposta `"Transportadoras buscadas no banco de dados."` |
| Dado recuperado do cache | Lista devolvida sem acessar o banco | Log `[CACHE] Transportadoras recuperadas do CACHE.` e mensagem `"Transportadoras recuperadas do cache."` |
| Cache invalidado | Ao cadastrar, alterar, ativar/desativar ou excluir uma transportadora, a chave é removida; a próxima consulta vai ao banco | Log `[CACHE] Cache de transportadoras INVALIDADO (dados alterados).` |

Na tela de Transportadoras, abaixo do título, aparece a origem dos dados (cache ou banco); o botão de recarregar permite demonstrar isso.

## Principais endpoints

Todos começam com `http://localhost:5080/api`. Todos exigem token, exceto o login. Os cadastros exigem perfil ADMIN ou GERENTE, mas as listas de produtos, armazéns, clientes, veículos e motoristas são liberadas para o OPERADOR, porque as telas de Estoque e Viagens usam. Estoque e viagens são liberados para todos.

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/autenticacao/login` | Login (público) — devolve o token JWT |
| PUT | `/autenticacao/senha` | Trocar a senha temporária no primeiro acesso |
| GET / POST | `/usuarios` | Listar / cadastrar usuário (ADMIN) |
| GET / PUT / DELETE | `/usuarios/{id}` | Buscar / alterar / excluir usuário (ADMIN) |
| PATCH | `/usuarios/{id}/status` | Ativar/desativar usuário (ADMIN) |
| GET / POST | `/transportadoras` | Listar (**com cache**) / cadastrar |
| GET / PUT / DELETE | `/transportadoras/{id}` | Buscar / alterar / excluir |
| PATCH | `/transportadoras/{id}/status` | Ativar/desativar |
| GET / POST | `/motoristas` | Listar / cadastrar |
| GET / PUT / DELETE | `/motoristas/{id}` | Buscar / alterar / excluir |
| GET / POST | `/veiculos` | Listar / cadastrar |
| GET / PUT / DELETE | `/veiculos/{id}` | Buscar / alterar / excluir |
| PATCH | `/veiculos/{id}/status` | Alterar status (`{ "status": "MANUTENCAO" }`) |
| GET / POST | `/clientes` | Listar / cadastrar |
| GET / PUT / DELETE | `/clientes/{id}` | Buscar / alterar / excluir |
| GET / POST | `/armazens` | Listar / cadastrar |
| GET / PUT / DELETE | `/armazens/{id}` | Buscar / alterar / excluir |
| GET / POST | `/produtos` | Listar / cadastrar |
| GET / PUT / DELETE | `/produtos/{id}` | Buscar / alterar / excluir |
| GET | `/estoque` | Saldo de cada produto em cada armazém |
| GET | `/estoque/movimentacoes` | Últimas 20 movimentações |
| POST | `/estoque/movimentacoes` | Registrar entrada ou saída |
| GET / POST | `/viagens` | Listar / iniciar viagem |
| PATCH | `/viagens/{id}/avancar` | Iniciada → em trânsito |
| PATCH | `/viagens/{id}/concluir` | Em trânsito → concluída |
| PATCH | `/viagens/{id}/cancelar` | Cancelar uma viagem em andamento |

## Exemplos de requisições

**Login**

```http
POST /api/autenticacao/login
Content-Type: application/json

{ "email": "admin@gestao.com", "senha": "SUA_SENHA" }
```

```json
{
  "sucesso": true,
  "mensagem": "Login realizado com sucesso.",
  "dados": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "usuario": { "id": 1, "nome": "Administrador", "email": "admin@gestao.com", "perfil": "ADMIN", "status": "ATIVO" }
  },
  "erros": []
}
```

**Endpoint protegido sem token**

```http
GET /api/produtos
```

```json
{
  "sucesso": false,
  "mensagem": "Acesso não autorizado.",
  "dados": null,
  "erros": ["Token ausente, inválido ou expirado. Faça login novamente."]
}
```

**Cadastrar motorista**

```http
POST /api/motoristas
Authorization: Bearer <token>
Content-Type: application/json

{
  "transportadoraId": 1,
  "nome": "João da Silva",
  "cpf": "123.456.789-00",
  "cnh": "MG1234567",
  "categoriaCnh": "E",
  "validadeCnh": "2028-05-10",
  "telefone": "(32) 99999-0000"
}
```

```json
{
  "sucesso": true,
  "mensagem": "Motorista cadastrado com sucesso!",
  "dados": { "id": 7, "transportadoraId": 1, "nomeTransportadora": "TransLog Sul", "nome": "João da Silva", "cnhVencida": false },
  "erros": []
}
```

**Registrar saída de estoque**

```http
POST /api/estoque/movimentacoes
Authorization: Bearer <token>
Content-Type: application/json

{ "produtoId": 1, "armazemId": 1, "tipo": "SAIDA", "quantidade": 5 }
```

```json
{
  "sucesso": true,
  "mensagem": "Movimentação registrada com sucesso!",
  "dados": { "produtoId": 1, "produto": "Notebook Dell Inspiron 15", "armazemId": 1, "armazem": "CD São Paulo", "quantidade": 40 },
  "erros": []
}
```

**Erro de validação**

```http
POST /api/autenticacao/login
Content-Type: application/json

{ "email": "abc", "senha": "" }
```

```json
{
  "sucesso": false,
  "mensagem": "Dados inválidos.",
  "dados": null,
  "erros": ["E-mail inválido.", "Informe a senha."]
}
```

## Requisitos do front-end (Web III)

- Páginas com endereço próprio via **Vue Router** (`/transportadoras`, `/motoristas`, `/veiculos`, `/clientes`, `/armazens`, `/produtos`, `/estoque`, `/viagens`, `/usuarios`).
- Estado em **stores Pinia** (uma por módulo, mais `auth` e `mensagem`).
- **Feedback** após toda operação: `v-snackbar` verde (sucesso) ou vermelho (erro), com a mensagem que a API devolveu.
- Usabilidade: tabelas `v-data-table` com busca, ordenação e paginação; formulários em janelas (`v-dialog`) com validação dos campos; confirmação antes de excluir; status com `v-chip` colorido; menu lateral; layout responsivo.
