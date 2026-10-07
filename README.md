# 🏠 ImóvelRadar

> Plataforma de inteligência imobiliária para coleta, normalização,
> histórico, análise de mercado e identificação de oportunidades.

O **ImóvelRadar** é um projeto em C#/.NET criado para transformar dados
de anúncios imobiliários em informações úteis para análise de mercado.

A proposta vai além de cadastrar ou coletar imóveis. O objetivo é
construir uma plataforma capaz de acompanhar anúncios ao longo do tempo,
calcular indicadores de mercado, identificar imóveis abaixo do preço da
região e manter rastreabilidade sobre a origem e as alterações dos
dados.

## 🎯 Objetivo

O projeto busca responder perguntas como:

-   Qual é o preço médio e mediano dos imóveis de uma região?
-   Qual é o preço médio por metro quadrado?
-   Quais imóveis estão abaixo do preço de mercado?
-   Um imóvel teve redução de preço?
-   Quanto o preço mudou desde a primeira observação?
-   Quantas vezes o preço foi alterado?
-   Há imóveis com potencial de oportunidade?
-   Qual é o nível de confiança de uma análise?
-   Quando um anúncio foi visto pela primeira e pela última vez?
-   Um anúncio deixou de aparecer em uma fonte?

O fluxo conceitual da plataforma é:

``` text
FONTES DE DADOS
      ↓
COLETA
      ↓
NORMALIZAÇÃO
      ↓
DEDUPLICAÇÃO
      ↓
PERSISTÊNCIA
      ↓
HISTÓRICO
      ↓
ANÁLISE ESTATÍSTICA
      ↓
OPORTUNIDADES
      ↓
ALERTAS
      ↓
DASHBOARD
```

## 🚧 Status atual

A primeira fase do projeto, focada em **domínio, regras de negócio e
arquitetura**, está concluída.

Atualmente o projeto possui:

-   modelagem de imóveis;
-   modelagem de fontes;
-   normalização de dados brutos;
-   importação individual e em lote;
-   deduplicação por fonte;
-   histórico de preços;
-   rastreabilidade de primeira e última observação;
-   estatísticas de mercado;
-   comparação entre imóveis;
-   cálculo de oportunidade;
-   classificação de confiança;
-   ranking de oportunidades;
-   paginação;
-   controle de execuções de coleta;
-   distinção entre coleta completa e parcial;
-   status observável do anúncio;
-   reconciliação de anúncios ausentes com escopo;
-   repositories em memória;
-   testes automatizados separados por camada.

A próxima fase é a implementação da persistência utilizando **Entity
Framework Core + SQL Server em ambiente local/dev**.

## 🧱 Arquitetura

O projeto segue uma separação em camadas:

``` text
ImovelRadar/
├── src/
│   ├── ImovelRadar.Api/
│   ├── ImovelRadar.Application/
│   ├── ImovelRadar.Domain/
│   └── ImovelRadar.Infrastructure/
├── tests/
│   ├── ImovelRadar.Domain.Tests/
│   ├── ImovelRadar.Application.Tests/
│   └── ImovelRadar.Infrastructure.Tests/
├── docs/
└── ImovelRadar.slnx
```

### Domain

Contém as entidades, enums e regras de negócio que não dependem de
infraestrutura.

Exemplos:

-   `Property`
-   `PropertyPriceHistory`
-   `Source`
-   `CollectionRun`
-   `CollectionScope`
-   serviços de estatística e análise
-   enums do domínio

### Application

Contém os casos de uso e contratos necessários para executar as
operações da aplicação.

Exemplos:

-   normalização;
-   importação;
-   importação em lote;
-   consultas;
-   paginação;
-   análise de oportunidades;
-   estatísticas de bairro;
-   reconciliação;
-   interfaces de repositories.

### Infrastructure

Contém as implementações concretas de infraestrutura.

Atualmente são utilizados repositories em memória, como:

-   `InMemoryPropertyRepository`
-   `InMemorySourceRepository`
-   repository de histórico em memória

Essa camada receberá posteriormente as implementações com Entity
Framework Core.

### API

Responsável pela exposição HTTP da aplicação e configuração de
Dependency Injection.

A lógica de negócio permanece fora dos controllers.

## 🏘️ Property

`Property` representa um imóvel observado em uma determinada fonte.

Entre os principais dados estão:

``` text
Id
Title
PropertyType
TransactionType
Price
Area
Bedrooms
Bathrooms
ParkingSpaces
CondoFee
Iptu
Address
Neighborhood
City
State
PostalCode
Latitude
Longitude
SourceId
ExternalId
SourceUrl
ObservationStatus
FirstSeenAt
LastSeenAt
CreatedAt
UpdatedAt
```

A entidade protege regras como:

-   preço deve ser maior que zero;
-   área, quando informada, deve ser maior que zero;
-   cidade é obrigatória;
-   estado é obrigatório;
-   fonte é obrigatória;
-   datas de observação não podem retroceder;
-   preço só pode ser atualizado para um valor válido.

Também é capaz de calcular:

``` text
Preço por m² = Preço / Área
```

## 👁️ Rastreabilidade do anúncio

O projeto diferencia três datas importantes:

``` text
FirstSeenAt
→ primeira vez que o anúncio foi conhecido pelo sistema

LastSeenAt
→ última vez que o anúncio foi observado na fonte

UpdatedAt
→ última alteração dos dados internos do imóvel
```

Essa separação permite saber que um anúncio continua existindo mesmo
quando nenhum dado foi alterado.

## 🟢 Status observável

O estado do anúncio é representado por:

``` text
Active
NotObserved
```

Uma regra importante do projeto é:

``` text
NotObserved ≠ Sold
```

Se um anúncio deixou de aparecer em uma coleta, o sistema **não afirma
que o imóvel foi vendido**.

O anúncio pode ter sido:

-   removido;
-   pausado;
-   expirado;
-   temporariamente indisponível;
-   vendido sem confirmação;
-   retirado da fonte por outro motivo.

Caso volte a aparecer posteriormente, ele pode ser marcado novamente
como `Active`.

## 🌐 Source

`Source` representa a origem dos anúncios.

Principais dados:

``` text
Id
Name
BaseUrl
Type
IsActive
CreatedAt
UpdatedAt
```

Tipos atualmente previstos:

``` text
Api
Csv
Manual
```

Uma fonte pode ser ativada ou desativada sem apagar seu histórico.

Credenciais, tokens e segredos de integração não pertencem à entidade
`Source`.

## 🔑 Deduplicação

A identidade externa de um anúncio é determinada pela combinação:

``` text
SourceId + ExternalId
```

Isso é necessário porque duas fontes diferentes podem utilizar o mesmo
identificador externo para anúncios distintos.

Exemplo:

``` text
Fonte 1 → ExternalId 123
Fonte 2 → ExternalId 123
```

Esses registros não são considerados duplicados.

Na futura persistência SQL, essa combinação deverá possuir um índice
único composto.

## 🧹 Normalização

Os dados coletados entram inicialmente como `RawProperty`.

Os campos são propositalmente recebidos como texto porque fontes
externas podem enviar valores como:

``` text
"R$ 650.000,00"
"100 m²"
"2 quartos"
"Venda"
"Apartamento"
```

O `PropertyNormalizer` converte esses valores para o modelo interno.

Entre as regras atuais:

-   validação de `SourceId`;
-   conversão de preço;
-   conversão de área;
-   conversão de quartos, banheiros e vagas;
-   normalização de estado;
-   remoção de espaços extras;
-   conversão de tipo de imóvel;
-   conversão de tipo de transação;
-   retorno estruturado de erros.

Dados inválidos retornam erros de normalização em vez de interromper um
lote inteiro por exceção.

## 📥 Importação

O `PropertyImportService` executa o fluxo:

``` text
RawProperty
     ↓
PropertyNormalizer
     ↓
validação
     ↓
SourceId + ExternalId
     ↓
┌─────────────────────┐
│ imóvel não existe   │ → Created
├─────────────────────┤
│ mesmo preço         │ → Unchanged
├─────────────────────┤
│ preço diferente     │ → Updated
├─────────────────────┤
│ dado inválido       │ → Rejected
└─────────────────────┘
```

Status disponíveis:

``` text
Created
Updated
Unchanged
Rejected
```

Quando um imóvel novo é criado, seu preço inicial também entra no
histórico.

Quando o preço não mudou:

-   `LastSeenAt` é atualizado;
-   nenhum novo histórico de preço é criado.

Quando o preço mudou:

-   o imóvel é atualizado;
-   `LastSeenAt` é atualizado;
-   um novo registro de histórico é criado.

Se um imóvel estava `NotObserved` e reaparece, ele volta para `Active`.

## 📦 Importação em lote

O `PropertyBatchImportService` permite processar vários `RawProperty`.

O resultado contém:

``` text
Total
Created
Updated
Unchanged
Rejected
Results
```

A implementação atual é sequencial de propósito.

A paralelização ficará para uma etapa futura, quando houver uma
estratégia segura de concorrência e deduplicação no banco.

## 📈 Histórico de preços

`PropertyPriceHistory` é tratado como histórico imutável.

Cada registro contém:

``` text
PropertyId
Price
CollectedAt
SourceId
```

Uma alteração de preço cria um novo registro em vez de modificar
registros históricos.

Exemplo:

``` text
01/01 → R$ 700.000
01/02 → R$ 680.000
01/03 → R$ 650.000
```

O `PriceHistoryAnalyzer` consegue calcular:

-   preço inicial;
-   preço atual;
-   menor preço;
-   maior preço;
-   variação percentual;
-   existência de redução;
-   quantidade real de mudanças de preço.

## 📊 Estatísticas de mercado

O `MarketStatisticsService` possui cálculos como:

-   média de preço por m²;
-   mediana de preço por m²;
-   diferença percentual entre um imóvel e o mercado.

Exemplo:

``` text
Imóvel: R$ 6.500/m²
Mercado: R$ 9.000/m²

Diferença ≈ -27,78%
```

Valores negativos indicam que o imóvel está abaixo da referência
utilizada.

Também existe análise estatística por bairro com indicadores como:

``` text
PropertyCount
AveragePrice
MedianPrice
AveragePricePerSquareMeter
MedianPricePerSquareMeter
LowestPrice
HighestPrice
```

## 🏘️ Comparáveis

O `PropertyComparisonService` seleciona imóveis comparáveis considerando
atualmente:

-   mesmo tipo de imóvel;
-   mesma transação;
-   mesma cidade;
-   mesmo estado;
-   mesmo bairro;
-   mesma quantidade de quartos.

O próprio imóvel analisado é excluído da comparação.

Essa regra poderá evoluir posteriormente com critérios como distância,
área aproximada, vagas, condomínio e características adicionais.

## 🎯 Opportunity Score

O projeto possui uma primeira versão do `OpportunityScoreService`.

O score atual utiliza a diferença do preço por m² em relação ao mercado:

  Diferença em relação ao mercado     Score
  --------------------------------- -------
  ≤ -20%                                100
  ≤ -15%                                 90
  ≤ -10%                                 80
  ≤ -5%                                  70
  \< 0%                                  60
  ≤ 5%                                   40
  \> 5%                                  20

O nível é classificado como:

``` text
Excellent
Good
Moderate
Low
```

Essa é uma heurística inicial e poderá receber novos fatores
futuramente.

## 🧠 Confiança da análise

O sistema também calcula um nível de confiança com base na quantidade de
imóveis comparáveis.

Regra atual:

  Comparáveis   Confiança
  ------------- -----------
  0--1          Low
  2--4          Moderate
  5--9          Good
  10+           High

Isso evita apresentar um score sem indicar a quantidade de evidência
disponível para sustentá-lo.

## 🏆 Ranking de oportunidades

O `OpportunityRankingService` combina:

``` text
Imóvel
+
Comparáveis
+
Preço por m² do mercado
+
Histórico de preços
+
Opportunity Score
+
Confidence Level
```

O ranking é ordenado globalmente antes da paginação.

Entre os dados retornados estão:

``` text
Price
PricePerSquareMeter
MarketPricePerSquareMeter
DifferenceFromMarket
ComparablePropertyCount
ConfidenceLevel
OpportunityScore
OpportunityLevel
HasPriceReduction
HistoricalVariationPercentage
```

## 🔄 CollectionRun

Uma `CollectionRun` representa uma execução de coleta de determinada
fonte.

Estados:

``` text
Running
Completed
Failed
```

Fluxos permitidos:

``` text
Running → Completed
Running → Failed
```

Uma execução finalizada não pode ser reaberta ou ter seu resultado
histórico alterado.

Também existem dois tipos:

``` text
FullSnapshot
Partial
```

### FullSnapshot

Representa uma coleta que afirma cobrir todo o universo definido pelo
seu escopo.

### Partial

Representa uma coleta parcial, paginação incompleta, delta ou qualquer
execução que não permita concluir que um anúncio ausente deixou de
existir na fonte.

## 🎯 CollectionScope

Uma execução pode possuir um escopo contendo:

``` text
City
State
Neighborhood
PropertyType
TransactionType
Bedrooms
```

Exemplo:

``` text
São Paulo
SP
Tatuapé
Apartment
Sale
2 quartos
```

Isso impede que uma coleta completa do Tatuapé altere um imóvel da
Mooca.

## 🔄 Reconciliação

A reconciliação é utilizada para identificar anúncios que deixaram de
ser observados.

Ela só é permitida quando:

``` text
CollectionRun.Type == FullSnapshot
AND
CollectionRun.Status == Completed
AND
CollectionRun.Scope != null
```

Exemplo:

``` text
Snapshot anterior:
A B C D

Snapshot atual:
A B C

Resultado:
A → Active
B → Active
C → Active
D → NotObserved
```

Mas apenas imóveis:

-   da mesma fonte;
-   dentro do mesmo escopo;
-   com identificador externo válido;

podem ser reconciliados.

Uma coleta parcial ou com falha **nunca** pode marcar imóveis ausentes
como `NotObserved`.

## 📄 Paginação

As consultas possuem suporte a:

``` text
Page
PageSize
TotalItems
TotalPages
HasPreviousPage
HasNextPage
```

O limite atual de página é de 100 itens.

Na implementação em memória, a filtragem acontece antes da paginação.

Quando o EF Core for introduzido, a paginação será movida para a
consulta ao banco utilizando `IQueryable`, `Skip` e `Take`.

## 🔌 Repositories

A Application depende de abstrações como:

``` text
IPropertyRepository
IPropertyPriceHistoryRepository
ISourceRepository
```

As implementações atuais utilizam memória.

A arquitetura permite futuramente trocar:

``` text
IPropertyRepository
        │
        ├── InMemoryPropertyRepository
        └── EfPropertyRepository
```

sem fazer a camada Application depender diretamente do SQL Server.

## 🧪 Testes

O projeto utiliza **xUnit**.

Os testes são separados por responsabilidade:

``` text
tests/
├── ImovelRadar.Domain.Tests/
├── ImovelRadar.Application.Tests/
└── ImovelRadar.Infrastructure.Tests/
```

Atualmente a suíte cobre, entre outros:

-   validações de `Property`;
-   atualização de preço;
-   cálculo de preço por m²;
-   `FirstSeenAt` e `LastSeenAt`;
-   status de observação;
-   `Source`;
-   normalização;
-   importação;
-   deduplicação;
-   histórico de preço;
-   importação em lote;
-   estatísticas;
-   comparação;
-   oportunidade;
-   confiança;
-   ranking;
-   paginação;
-   `CollectionRun`;
-   snapshots completos e parciais;
-   escopo;
-   reconciliação.

Para executar:

``` powershell
dotnet test
```

O projeto mantém como regra que alterações importantes devem preservar a
suíte de testes verde.

## 🚀 Executando o projeto

### Requisitos atuais

-   .NET SDK 10
-   Git

Clone o repositório e entre na pasta:

``` powershell
git clone <URL-DO-REPOSITORIO>
cd ImovelRadar
```

Restaure as dependências:

``` powershell
dotnet restore
```

Compile:

``` powershell
dotnet build
```

Execute os testes:

``` powershell
dotnet test
```

Execute a API:

``` powershell
dotnet run --project .\src\ImovelRadar.Api\ImovelRadar.Api.csproj
```

> Neste estágio, a aplicação ainda utiliza repositories em memória. A
> configuração de SQL Server será adicionada na próxima fase.

## 🗄️ Próxima fase: EF Core + SQL Server

A próxima etapa do projeto será a persistência real.

Planejamento:

``` text
Instalar Entity Framework Core
        ↓
Criar ImovelRadarDbContext
        ↓
Criar EntityTypeConfigurations
        ↓
Configurar relacionamentos
        ↓
Configurar índices
        ↓
Configurar SQL Server local/dev
        ↓
Criar migration InitialCreate
        ↓
Criar banco
        ↓
Implementar repositories EF Core
        ↓
Substituir repositories InMemory no DI
```

As primeiras tabelas previstas são:

``` text
Sources
Properties
PropertyPriceHistories
CollectionRuns
```

Relacionamentos principais:

``` text
Source 1 ───── N Property

Source 1 ───── N CollectionRun

Property 1 ─── N PropertyPriceHistory
```

Também está previsto um índice único para:

``` text
(SourceId, ExternalId)
```

O `CollectionScope` deverá ser persistido como parte da execução da
coleta, evitando uma tabela desnecessária apenas para o escopo.

## 🛣️ Roadmap

Depois da persistência, algumas evoluções planejadas são:

-   repositories com EF Core;
-   consultas e paginação executadas diretamente no banco;
-   dataset de demonstração persistente;
-   endpoints adicionais;
-   tratamento global de erros;
-   remoção de endpoints/template não utilizados;
-   melhorias no Opportunity Score;
-   critérios mais avançados para imóveis comparáveis;
-   geolocalização e análise por distância;
-   detecção de tempo no mercado;
-   análise histórica mais avançada;
-   alertas de oportunidades;
-   dashboard em React/TypeScript;
-   execução automatizada de coletas autorizadas;
-   métricas de fontes e execuções;
-   otimizações de ranking;
-   eventual integração com IA através de APIs controladas.

## ⚠️ Princípios do projeto

Algumas decisões são intencionais:

**Ausência não significa venda.**\
Um anúncio que deixou de aparecer é marcado como `NotObserved`, não como
vendido.

**Histórico não é sobrescrito.**\
Alterações de preço geram novos registros históricos.

**Fontes são substituíveis.**\
A lógica de negócio não deve depender de um portal ou fornecedor
específico.

**Coletas devem respeitar autorização e termos da fonte.**\
HTML público não é tratado automaticamente como uma API.

**Controllers não carregam regras de negócio.**\
As regras ficam no Domain/Application.

**Banco não deve ser acessado diretamente por toda a aplicação.**\
O acesso é abstraído por repositories.

**Segredos não pertencem ao código-fonte.**\
Tokens, senhas e credenciais não devem ser versionados no Git.

**Análises não devem inventar fatos.**\
Estimativas, oportunidades e inferências devem permanecer distintas de
fatos observados.

## 🛠️ Tecnologias

Atualmente:

-   C#
-   .NET 10
-   ASP.NET Core
-   xUnit
-   Git

Planejado:

-   Entity Framework Core
-   SQL Server
-   React
-   TypeScript
-   Docker
-   Azure

## 📌 Escopo inicial

O primeiro nicho do projeto está focado em:

``` text
Tipo: Apartamento
Transação: Venda
Região inicial: São Paulo
Perfil inicial: 2 quartos
```

A arquitetura foi construída para permitir expansão futura sem acoplar o
domínio a esse recorte.

## 📚 Filosofia

O ImóvelRadar não pretende ser apenas um coletor de anúncios.

A coleta é apenas a entrada.

O valor da plataforma está em transformar observações ao longo do tempo
em inteligência:

``` text
Dado bruto
   ↓
Dado confiável
   ↓
Histórico
   ↓
Contexto de mercado
   ↓
Análise
   ↓
Oportunidade
```

------------------------------------------------------------------------

## 📄 Licença

Defina a licença do projeto antes de disponibilizá-lo publicamente. Caso
o repositório seja apenas para estudo ou portfólio, esta seção pode ser
ajustada conforme a estratégia de publicação.
