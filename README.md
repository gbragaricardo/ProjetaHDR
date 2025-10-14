# ProjetaHDR — Documentação completa (Português — Brasil)

Este README descreve, em detalhe, cada pasta, namespace e CADA CLASSE visível no código fornecido do repositório ProjetaHDR. Mantém os nomes (classes, métodos, propriedades, parâmetros) exatamente como aparecem no código. O objetivo é permitir que qualquer desenvolvedor que assuma o projeto entenda o propósito, o comportamento, as dependências e as observações de implementação de cada unidade do sistema.

Sumário
- Visão geral
- Estrutura de pastas e namespaces
- Documentação por classe (ordem alfabética por namespace e arquivo)
- Como rodar / Debug no Visual Studio
- Observações importantes e boas práticas
- Itens referenciados mas não incluídos no snapshot

Visão geral
----------
ProjetaHDR é um add-in para Autodesk Revit focado em utilitários hidráulicos: criação de tags, gestão de parâmetros aninhados, exportação de documentos Word (memorial descritivo), e ferramentas para cálculo de redes pluviais. O plugin cria uma aba chamada `ProjetaHDR` na Ribbon do Revit e fornece janelas WPF para interação do usuário. Algumas funcionalidades usam Microsoft Office Interop (Word) — requer Word instalado.

Estrutura de pastas e namespaces (visíveis)
- RevitAddin\OnStartup — namespace `ProjetaHDR.Startup`
  - UIBuilder.cs — monta a Ribbon e botões
  - RibbonManager.cs — helper para criar painéis e botões da Ribbon
- RevitAddin\Commands — namespace `ProjetaHDR.Commands` (subpastas por tema)
  - Parameters — comandos que manipulam parâmetros (ex.: `Seduc.cs`)
  - Memorials — comandos relacionados a export/Word (`MemoHDS.cs` + serviços)
  - Tags — serviços para filtrar e taggear tubos
  - RainNetwork, Dev — comandos que abrem janelas/handlers
- RevitAddin\Commands\Tags\Services — serviços para tagging e pipeline (PipeMethods, TagManager, etc.)
- RevitAddin\Commands\Parameters\Services — `NestedFittings`
- RevitAddin\Utils — utilitários auxiliares (PipeUtils, BasicUtils, ViewDirections)
- UI\ViewModels — ViewModels para janelas WPF (WordExportViewModel, DrenViewModel, LoginViewModel)
- UI\Events — classes que implementam `IExternalEventHandler` para executar ações seguras no modelo Revit
- UI\Services — helpers (AuthService, SaveImageNamesToFile)
- RevitAddin\Commands\Memorials\Services — `WordHandler`, `DocHandler`, `Sheets`
- Root
  - AddinApp.cs — IExternalApplication entry point
  - guia-iniciar-plugin.txt — instruções de setup

Documentação por classe
----------------------
Abaixo cada classe com propósito, o que faz, como faz, entradas/saídas, dependências e observações.

AddinApp (namespace raiz)
- Arquivo: AddinApp.cs
- O que é: Implementação de `Autodesk.Revit.UI.IExternalApplication`.
- O que faz:
  - `OnStartup(UIControlledApplication application)` chama `AddinAppLoader.StartupMain(application)` dentro de try/catch e mostra `TaskDialog` em caso de erro.
  - `OnShutdown(UIControlledApplication application)` apenas retorna `Result.Succeeded`.
- Como faz: delega a inicialização real para `AddinAppLoader` (não incluída no snapshot) — espera-se que lá esteja a chamada a `UIBuilder.BuildUI`.
- Observações: `OnStartup` retorna `Result.Succeeded` mesmo se a inicialização falhar (o erro é mostrado via TaskDialog). Pode-se melhorar liberando recursos no shutdown se necessário.

UIBuilder (namespace ProjetaHDR.Startup)
- Arquivo: RevitAddin\OnStartup\UIBuilder.cs
- O que é: Construtor estático da UI do add-in (Ribbon).
- O que faz:
  - `BuildUI(UIControlledApplication application)` cria painéis (tabs) e adiciona botões, split buttons e stacked buttons chamando métodos de `RibbonManager`.
  - Define labels, tooltips, icons e classes de comando (strings que apontam para `ProjetaHDR.Commands.*`).
- Como faz:
  - Invoca `RibbonManager.CriarRibbonPanel` para criar painéis: Login, Main, Tabelas, Docs, Drenagem.
  - Para cada comando usa `CreateAndAddPushButton` ou `CreatePushButtonData` + `AddSplitButton`/`AddStackedPushButtons`.
  - Botões passados com `ThisAssemblyPath` (na RibbonManager) permitem ao Revit instanciar os `IExternalCommand` via reflection.
- Observações: nomes de imagem (ex.: `"eggprojeta.png"`) devem existir no output; utiliza espaços Unicode para alinhamento visual.

RibbonManager (namespace ProjetaHDR.Startup)
- Arquivo: RevitAddin\OnStartup\RibbonManager.cs
- O que é: Helper estático para criar painéis e botões Ribbon e manter referências para controle posterior.
- O que faz:
  - `CriarRibbonPanel` cria painel e captura exceções.
  - `CreateAndAddPushButton` cria e adiciona um `PushButton` ao painel, define tooltip, ícone (`ResourceImage.GetIcon`) e adiciona à lista `PushButtonsList`.
  - `AddStackedPushButtons` cria botões empilhados (stacked) a partir de `PushButtonData` e desabilita por padrão.
  - `AddSplitButton` cria `SplitButton` e o adiciona à lista `SplitButtonList`.
  - `CreatePushButtonData` cria `PushButtonData` reutilizável com ícone e tooltip.
- Como faz:
  - Usa `Assembly.GetExecutingAssembly().Location` (field `ThisAssemblyPath`) para apontar o assembly que contém os handlers de comando.
  - Retorna objetos `PushButton`, `SplitButton` para configurações adicionais (visible/enable).
- Observações: lê ícones via `ResourceImage.GetIcon(string)` (helper externo). Se `painel` for `null` escreve no `Debug`.

RevitCommandBase (assinatura)
- Local: assinatura disponível no snapshot.
- O que é: classe base abstrata usada por comandos (`Seduc`, `MemoHDS`, etc).
- O que faz:
  - Expõe `protected RevitContext Context { get; set; }`.
  - Métodos `InitializeContext(ExternalCommandData)` e `InitializeContextEvent(UIApplication)` para popular `Context`. Implementação real não incluída, mas usada por comandos.
- Observações: garante padrão para inicializar contexto Revit (UiApp, UiDoc, Doc, ActiveView).

RevitContext (assinatura)
- O que é: container para referências Revit (UIApplication, UIDocument, Document, View).
- Uso: passado a ViewModels e handlers que precisam do documento ativo.

Seduc (namespace ProjetaHDR.Commands, arquivo RevitAddin\Commands\Parameters\Seduc.cs)
- O que é: comando (`IExternalCommand`) que sincroniza parâmetro compartilhado "Etapa Seduc" do host para famílias aninhadas.
- O que faz:
  - `Execute(ExternalCommandData commandData, ref string message, ElementSet elements)` inicializa Context, abre `Transaction` "Etapa Seduc", chama `ModifiedFamilies()` e exibe um `TaskDialog` com o resultado.
  - `ModifiedFamilies()` recolhe elementos das categorias: `OST_PipeFitting`, `OST_PipeCurves`, `OST_PipeAccessory`, `OST_PlumbingFixtures` via `PipeUtils.GetAllOfCategory` e aplica `MatchNestedParams` em cada conjunto.
  - `MatchNestedParams(IList<Element> category, int counter)` itera elementos; para cada `FamilyInstance` obtém `SuperComponent` (família hospedeira). Usa GUID fixo `b0e00dee-3122-4875-bf2d-1aa3dc310003` para obter parâmetro no host e na instância aninhada e copia valor (AsString → Set) se diferente.
- Como faz: modifica parâmetros dentro de uma `Transaction`. Busca por GUID (parâmetro compartilhado), assume tipo string. Usa `Parameter.AsString()` e `Parameter.Set(string)`.
- Observações e riscos:
  - Se `Set` falhar (param read-only, tipos incompatíveis) pode lançar e abortar a transação — o código não trata exceções item-a-item.
  - Requer que o GUID esteja correto e o parâmetro exista nas famílias.

NestedFittings (namespace raiz)
- Arquivo: RevitAddin\Commands\Parameters\Services\NestedFittings.cs
- O que é: utilitário estático para copiar parâmetros por nome de família hospedeira para aninhadas.
- O que faz:
  - `InserirSistemaFamiliaAninhada(IList<Element> instancias, string nomeParametroHospedeiro, string nomeParametroAninhado)` itera `instancias`, para cada `FamilyInstance` com `SuperComponent`, usa `LookupParameter(nomeParametroHospedeiro)` no host e `LookupParameter(nomeParametroAninhado)` no aninhado e copia o valor se diferente.
- Como faz: usa `LookupParameter` (por nome) em vez de GUID. Retorna contador de alterações.
- Observações: usado por comando `SanFittings` (referenciado no UI). Mantém lógica análoga ao `Seduc` mas por nomes de parâmetro.

Dev (namespace ProjetaHDR.Commands)
- Arquivo: RevitAddin\Commands\Dev.cs
- O que é: comando placeholder para modo desenvolvedor.
- O que faz: `Execute` retorna `Result.Succeeded` sem ação.
- Observações: botão `Dev` é criado no UI e inicialmente invisível — ativado pelo login com usuário dev.

RainNetwork (namespace ProjetaHDR.Commands)
- Arquivo: RevitAddin\Commands\RainNetwork.cs
- O que é: comando que abre a janela da ferramenta de Rede Pluvial (Drenagem).
- O que faz:
  - Inicializa Context (via `InitializeContext`) e mantém `HelperContext`.
  - Garante que `DrenViewModel` e `DrenWindow` representam o documento atual (se o doc mudou, recria).
  - Se `ViewModel` nulo, cria `new DrenViewModel(Context)`.
  - Popula `ViewModel` e abre/fecha a janela `DrenWindow` (WPF). Usa `WindowInteropHelper` para associar a janela ao handle do Revit (`Context.UiApp.MainWindowHandle`).
- Observações: mantém instâncias estáticas `ViewModel` e `Window` para evitar re-criação desnecessária e preservar estado entre aberturas.

MemoHDS (namespace ProjetaHDR.Commands)
- Arquivo: RevitAddin\Commands\Memorials\MemoHDS.cs
- O que é: comando que abre UI para exportar Memorial Descritivo (Word).
- O que faz:
  - `Execute` inicializa contexto, cria `WordExportViewModel` e `WordExportWindow`, verifica `ExportPath` e mostra diálogo (`ShowDialog`).
- Observações: implementação do `WordExportWindow` e XAML não incluídos no snapshot, mas ViewModel manipula `WordHandler` para alteração do documento Word.

TagManager (namespace raiz)
- Arquivo: RevitAddin\Commands\Tags\Services\TagManager.cs
- O que é: serviços para localizar tipos de tag, criar tags e deletar tags em uma view.
- O que faz:
  - `GetTagId(Document doc, string tagMode)` busca tipo de tag (`OST_PipeTags`) cujo Name == tagMode e retorna `ElementId`.
  - `CreateTags` (2 overloads) cria `IndependentTag` para cada elemento da lista, usando um `insertionPoint` correspondente; valida se view 3D está travada (locked) antes de criar.
  - `DeleteExistingTags` identifica `IndependentTag` no `activeView` do mesmo `tagTypeId` e que tagueiam qualquer `Element` da lista `elementsList`; exclui em lote via `doc.Delete(listOfIds)`.
- Como faz: usa `FilteredElementCollector` para localizar tags, e `IndependentTag.Create` para criar. Para exclusões monta hashset de ids de elementos alvo e verifica `tag.GetTaggedLocalElementIds()`.
- Observações: para criar tags em 3D requer view travada (requisito Revit). `GetTagId` retorna `null` se não encontrar e o código chamador mostra `TaskDialog`.

FilterAndTagPipelines (namespace raiz)
- Arquivo: RevitAddin\Commands\Tags\Services\FilterAndTagPipelines.cs
- O que é: orquestrador para filtrar listas de tubos, calcular posições de inserção de tags e executar criação/exclusão de tags.
- Propriedades importantes: `Pipes`, `InsertPoints`, `RelativePosition`, `TagId`, `TagsIds`, `Doc`, `TagMode`, `IsHydraulic`, `LengthOption`, `FlowDirections`, `ViewDirections`.
- Métodos:
  - Construtor recebe `Document`, `View`, `tagMode`, `lengthOption`, `isHydraulic` e cria `ViewDirections`.
  - `PipelineHydraulic` / `PipelineSanitary` aplicam filtros via `PipeUtils` (por comprimento, por paralelismo/verticalidade).
  - `PipelineCreate` — se `TagMode == "Inclinacao"` chama `PipeMethods.SetPipeSlope`; obtém `RelativePosition` e `InsertPoints` via `PipeMethods`; obtém `TagId` via `TagManager.GetTagId`; deleta tags existentes (TagManager.DeleteExistingTags) e cria novas tags (TagManager.CreateTags).
  - `PipelineFlow` — calcula `FlowDirections` (via `PipeMethods.GetPipeFlow`), monta `TagsIds` para cada direção, deleta e cria tags por tipo.
- Observações: apresenta mensagens via `TaskDialog` quando parâmetros/tags não encontrados.

PipeMethods (namespace raiz)
- Arquivo: RevitAddin\Commands\Tags\Services\PipeMethods.cs
- O que é: utilitários computacionais para tubos (slope, posição relativa, pontos de inserção e fluxo).
- Métodos:
  - `SetPipeSlope(IList<Element> pipesList)` — atribui parâmetro `"PRJ HDR: Inclinacao Tag"` baseado na abreviatura do sistema e classificação, diâmetro (usa BuiltInParameter para obter diâmetro). Retorna false se todos os pipes não possuírem o parâmetro ou este for somente leitura; true caso contrário.
  - `GetRelativeViewPosition` — determina se cada tubo é Horizontal, Vertical ou Diagonal (usando projeção do vetor do tubo em `ViewDirections`).
  - `GetTaginsertPoint` — calcula ponto médio do tubo e aplica offset (meio diâmetro) transformado por `HelperMethods.AnalyzeOffset` para obter `InsertPoints`.
  - `GetPipeFlow` — para cada `Pipe` chama `HelperMethods.AnalyzePipeFlow` para determinar string de direção do fluxo.
- Observações: usa `ViewDirections` e `HelperMethods`; depende de parâmetros built-in do Revit.

HelperMethods (namespace raiz)
- Arquivo: RevitAddin\Commands\Tags\Services\HelperMethods.cs
- O que é: helpers geométricos e de análise de fluxo.
- Métodos:
  - `AnalyzeOffset(double offset, string planPosition, string tagMode, ViewDirections viewDirections)` — retorna vetor de offset (XYZ) dependendo de posição relativa e tipo de tag (Diametro vs outros).
  - `AnalyzePipeFlow(Pipe pipe, string tempDirection, bool IsHydraulic, ViewDirections viewDirections)` — determina direção "Direita" / "Esquerda" / fallback, usando conectores do pipe (FlowDirectionType.In/Out) se disponíveis; analisa vetores no sistema da vista se `IsHydraulic`, caso contrário usa coordenadas X/Y globais.
- Observações: trata exceções internamente e retorna `tempDirection` como fallback.

ViewDirections (namespace ProjetaHDR.Utils)
- Arquivo: RevitAddin\Utils\ViewDirections.cs
- O que é: encapsula vetores direcional da vista (Right, Left, Up, Down).
- O que faz:
  - Construtor `ViewDirections(View view)` normaliza e calcula vetores `Right`, `Left`, `Up`, `Down`.
  - Se `view` for `View3D`, usa `ViewDirection.CrossProduct(Right)` para garantir perpendicularidade entre Up e Right.
- Observações: usado por `PipeMethods`, `FilterAndTagPipelines` e `PipeUtils` para cálculos dependentes da orientação da vista.

PipeUtils (namespace ProjetaHDR.Utils)
- Arquivo: RevitAddin\Utils\PipeUtils.cs
- O que é: filtros e coletores utilitários para elementos tubulares e acessórios.
- Métodos:
  - `GetPipesOnView(Document doc, View activeView = null)` — `FilteredElementCollector` de `OST_PipeCurves` na view.
  - `FilterByLength(IList<Element> pipes, double lengthOption)` — filtra por `CURVE_ELEM_LENGTH` convertendo unidades.
  - `RemoveSanitaryVerticals(IList<Element> pipes)` — remove tubos verticais baseando-se nos pontos finais.
  - `RemoveViewParallels(IList<Element> pipes, ViewDirections viewDirections)` — remove tubos paralelos à vista (threshold).
  - `HasPvcMarromPipes`, `GetPvcMarromPipes` — identifica tipos cujo `ElementType.Name == "PVC Marrom Soldável"`.
  - `GetAllPipeFittings`, `GetAllOfCategory` — collectors para fittings e categorias genéricas.
- Observações: assume que `Location` do elemento é `LocationCurve`; não trata `null` em algumas conversões.

BasicUtils (namespace ProjetaHDR.Utils)
- Arquivo: RevitAddin\Utils\BasicUtils.cs
- O que é: utilitário de capitalização de texto.
- Método:
  - `Captalize(string texto)` (note o nome: Captalize) transforma texto em Title Case respeitando palavras ignoradas ("de","da",...).
- Observações: não trata `null`, múltiplos espaços ou palavras vazias. Nome contém typo — cuidado ao renomear.

LoginViewModel (namespace ProjetaHDR.UI.ViewModels)
- Arquivo: UI\ViewModels\LoginViewModel.cs
- O que é: ViewModel para tela de login do plugin (WPF).
- Propriedades:
  - `Username` (default "Hidro"), `Message`, `IsLoggedOff` (bool), `ImagesPath` (BitmapImage), `CloseWindow` action.
  - `LoginCommand` (RelayCommand) — executa `ExecuteLogin`.
- O que faz:
  - `ExecuteLogin(object parameter)` obtém senha via `PasswordBox` (cast) e chama `AuthService.Authenticate`.
  - Se sucesso: ajusta `IsLoggedOff=false`, `Message` e chama `EnableUI()`.
  - `EnableUI()` inicia `Transaction` "Ativar Botões" e itera `RibbonManager.PushButtonsList` e `SplitButtonList`, habilita/torna visível botões. Usuário `dev` habilita inclusive o botão `Dev`.
- Observações: altera visibilidade/enable de botões dentro de uma `Transaction` (isso é relativamente incomum — alteração de UI não precisa de transação Revit; mas o código envolve uma Transaction possivelmente para alterações de parâmetros ou para manter sincronização com estado do documento).

AuthService (namespace ProjetaHDR.UI.Services)
- Arquivo: UI\Services\AuthService.cs
- O que é: valida credenciais localmente (hardcoded).
- O que faz:
  - `Authenticate(string username, string password)` retorna true para combos hardcoded:
    - "Hidro" / "Pjt@2025"
    - "Dev" / "eddevmode"
  - Caso contrário retorna false.
- Observações: mecanismo estático e inseguro — apenas para uso local/dev.

DrenViewModel (namespace ProjetaHDR.UI.ViewModels)
- Arquivo: UI\ViewModels\DrenViewModel.cs
- O que é: ViewModel da ferramenta de Rede Pluvial (DrenWindow).
- Principais responsabilidades:
  - Gerir coleções de `FixtureFamilyItem` (AddedFixtureFamilies), seleção de fixtures/áreas, comandos de UI (adicionar/remover/mover/selecionar pipes), calcular vazões e persistir configuração via `SaveDataStorageEvent` (ExternalEvent).
- Principais métodos:
  - `AutoCalcFlowRate()` chama `UpdateFlowRate()` em cada fixture.
  - `SelectOutputPipes(object param)` — permite ao usuário selecionar pipes na vista (`Selection.PickObjects` com filtro `BuiltInCategory.OST_PipeCurves`), converte referências em `Pipe` e atualiza `SelectedFixtureFamily.OutputPipes`, chama `UpdateFlowPerConductor()` e `UpdateClassifiedOutputPipes()`.
  - `UpdateClassifiedOutputPipes()` — agrupa `OutputPipes` por `Name`, remove "PVC", converte diâmetro para mm e adiciona strings descritivas na coleção `ClassifiedOutputPipes`.
  - Métodos Add/Remove para InputFixture e InputArea ajustam seleção e re-calculam vazões conforme necessário.
  - `LoadFixturesFromRevit()` chama `FixtureStorageManager.LoadDataFromRevit(Context.Doc)` e reconstrói relacionamentos entre `InputFixtureItem.CorrespondentFixture`.
  - `GetPlumbingFixtures()` e `GetAllAreas()` usam `FilteredElementCollector` e parameters (GUIDs e BuiltInParameter.ROOM_NAME) para popular dicionários de elementos.
  - `SaveDataStorage()` dispara `_saveStorageEvent.Raise()` (executa `SaveDataStorageEvent` externamente no contexto Revit).
- Observações:
  - Usa `ExternalEvent` / handlers (`SaveDataStorageEvent`, `ExecuteDrenEvent`) para executar modificações que precisam do contexto Revit fora da thread de UI.
  - Tipos `FixtureFamilyItem`, `AreaFamilyItem`, `InputFixtureItem`, `FixtureStorageManager` são cruciais para persistência/estrutura — ver assinaturas abaixo.

ExecuteDrenEvent (namespace ProjetaHDR.UI.Events)
- Arquivo: UI\Events\ExecuteDrenEvent.cs
- O que é: `IExternalEventHandler` que aplica valores calculados no modelo Revit (flow rates e parâmetros nos pipes e equipamentos).
- O que faz:
  - Construtor recebe `Document doc` e `ObservableCollection<FixtureFamilyItem> mainFixtures`.
  - `Execute(UIApplication app)` inicializa contexto (`InitializeContextEvent(app)`), define GUIDs dos parâmetros (`flowRateParamGuid`, `conductorFlowParamGuid`, `sectionConductorsParamGuid`).
  - Abre `Transaction("Executar Rede Pluvial")`, itera as fixtures adicionadas, para cada fixture valida `InstanceElementId` e seta:
    - parametro fluxo no equipamento (`addedFixElement.get_Parameter(flowRateParamGuid).Set(addedFix.FlowRate)`).
    - calcula `sectionConductors` como string agregada (ex.: "2x Ø100mm") e seta em pipes.
    - para cada `pipe` seta `flowRateParamGuid` e `sectionConductorsParamGuid`; se houver pipes e `FlowRate > 0` calcula `conductorFlow = addedFix.FlowRate / addedFix.OutputPipes.Count()` e seta `conductorFlowParamGuid`.
  - Exibe `MessageBox.Show("Execução Completa!", "Retorno")` e `transaction.Commit()`.
- Observações:
  - Executa alterações diretamente no modelo dentro de `Transaction`.
  - Depende de `FixtureFamilyItem` conter `OutputPipes` e `FlowRate`.
  - Usa `get_Parameter(Guid).Set(...)` e assume compatibilidade de tipos (double/string).

SaveDataStorageEvent (assinatura)
- O que é: `IExternalEventHandler` responsável por persistir `AddedFixtureFamilies` no documento (via `FixtureStorageManager` ou outro mecanismo). Implementação não incluída no snapshot, mas é referenciada no `DrenViewModel`.

FixtureFamilyItem, AreaFamilyItem, InputFixtureItem (assinaturas)
- O que são: modelos/ViewModel items que representam:
  - FixtureFamilyItem — equipamento de saída com propriedades: `Id`, `IsValid`, `Description`, `InstanceElementId`, `IsSelected`, `FlowRate`, `FlowPerConductor`, `InputAreas`, `InputFixtureItems`, `OutputPipes`. Métodos: `UpdateFlowRate()`, `UpdateFlowPerConductor()`.
  - AreaFamilyItem — área com `Id`, `Description`, `InstanceElementId`, `FlowRate`, `IsSelected`, `UpdateAreaFlow()`.
  - InputFixtureItem — item referenciando fixture de entrada com `Id`, `CorrespondentFixture` (FixtureFamilyItem), `IsSelected`.
- Observações: implementações completas não presentes no snapshot, mas assinaturas indicam responsabilidades usadas por `DrenViewModel` e `ExecuteDrenEvent`.

FixtureStorageManager (referenciado)
- O que faz (esperado): serializar/deserialize `AddedFixtureFamilies` no/fro documento Revit (provavelmente via ProjectParameters, extensible storage ou shared parameters). Implementação não inclusa; `DrenViewModel.LoadFixturesFromRevit()` chama `FixtureStorageManager.LoadDataFromRevit(Context.Doc)`.

UI\ViewModels\WordExportViewModel (arquivo UI\ViewModels\WordExportViewModel.cs)
- O que é: ViewModel para manipular export do template Word (Memorial).
- O que faz:
  - `ExportPath` obtido via `DocHandler.GetSavePath()`; `DocHandler.LoadDocument(ExportPath)` copia template base para destino.
  - `ExportCommand` executa `Replace()` que:
    - Valida `InputCidade`, `InputEstado`.
    - Cria `WordHandler` passando `ProjectInfo` do Revit e `ExportPath`.
    - Usa `Sheets` para detectar `titleBlock` e `consorcioFullName`.
    - Abre documento (`handler.OpenWordDocument()`), executa substituições via `ReplaceText`, `ReplaceTextInFooter`, substitui imagens (se existirem em Resources) via `ReplaceImage`, `ReplaceFooterImage`, `ReplaceTableImage`.
    - Salva e fecha com `handler.SaveAndClose()` e abre documento com `DocHandler.OpenDocument(ExportPath)`.
- Observações: `WordHandler` encapsula Interop; `Sheets` mapeia titleblocks para nome de consórcio (usado para selecionar imagens).

WordHandler (namespace raiz, arquivo RevitAddin\Commands\Memorials\Services\WordHandler.cs)
- O que é: wrapper Disposable para Microsoft.Office.Interop.Word que realiza buscas/substituições e manipulação de imagens no documento.
- Principais funcionalidades:
  - `OpenWordDocument()` — cria `Word.Application`, abre `_wordDoc` e mantém referências.
  - `ReplaceText(string textoAntigo, string parametroReferencia)` — busca parâmetro no `ProjectInfo` (passed in constructor), define `textoNovo` e chama `FindAndReplace` em cada `StoryRanges`.
  - `FindAndReplace` — usa `Range.Find` repetidamente; para cada ocorrência chama `AdjustNewTextCase(foundText, newText)` (usa `BasicUtils.Captalize` quando apropriado) e substitui, destacando (`HighlightColorIndex`).
  - `ReplaceTextInFooter` — busca e substitui primeira ocorrência no rodapé (seção 2).
  - `ReplaceImage`, `ReplaceFooterImage`, `ReplaceTableImage` — localizam `Shape` por `AlternativeText`, removem e adicionam nova imagem respeitando `Anchor`, posição, tamanho, wrap e z-order.
  - `DeleteTags`, `DeleteSpecificParagraph` — remove marcadores de bloco (tags) no documento por find/replace (wildcards).
  - `SaveAndClose()` / `ExceptionClose()` — salvam/fecham e liberam COM objects via `Marshal.ReleaseComObject`.
  - `Dispose()` — existe mas sem implementação (classe implementa IDisposable; ideal implementar Release também).
- Observações e riscos:
  - Uso de Interop exige Word instalado.
  - `Sections[2]` é indexado diretamente — pode lançar se document tiver menos seções.
  - `Dispose()` vazio é um smell: liberar explicitamente `_wordDoc`/`_wordApp` em `Dispose` é recomendado.
  - `AdjustNewTextCase` tenta preservar caixa (upper/lower/title).

DocHandler (namespace raiz)
- Arquivo: RevitAddin\Commands\Memorials\Services\DocHandler.cs
- O que é: manipulação de arquivos (caminhos templates e operações de cópia/abrir).
- O que faz:
  - `RootPath` aponta para `Docs/mmd.docx` dentro da pasta do assembly.
  - `GetSavePath()` abre diálogo `Microsoft.Win32.SaveFileDialog` e retorna caminho salvo pelo usuário (`NewPath`) ou `null`.
  - `LoadDocument(string exportPath)` copia `RootPath` para `exportPath` (overwrite true) — mantém base original.
  - `OpenDocument(string exportPath)` usa `Process.Start` com `UseShellExecute = true` para abrir o arquivo salvo.
- Observações: verifica existência de `RootPath`; se não existir exibe `TaskDialog`.

Sheets (namespace raiz)
- Arquivo: RevitAddin\Commands\Memorials\Services\Sheets.cs
- O que é: detecta title block e mapeia para nome do consórcio.
- O que faz:
  - `GetTitleBlockName(Document doc)` — coleta `ViewSheet`, pega o primeiro, pesquisa titleblock (FamilyInstance em BuiltInCategory.OST_TitleBlocks), retorna `FamilyName`.
  - `ValidateTitleBlock(string titleBlockName)` — verifica se o nome contém palavras-chave (Diamante, Metaverso, Minas, Objetiva, Pitagoras, Projeta, Vitoria) e preenche `Consorcio` com a chave encontrada (lowercase). Retorna a string de nome completo do consórcio ou `null`.
- Observações: substitui acentos (á->a) ao retornar o `FamilyName`.

SaveImageNamesToFile (namespace ProjetaHDR.UI.Services)
- Arquivo: UI\Services\SaveImageNamesToFile.cs
- O que é: utilitário que abre um arquivo Word via Interop e extrai nomes/ids/alt text de imagens para um TXT.
- O que faz:
  - `ExecuteTESTE(string wordFilePath, string outputTxtPath)` abre Word, percorre `InlineShapes` e `Shapes`, coleta `AlternativeText` e `AnchorID`/`ID` e escreve linhas num arquivo de texto; abre o notepad com o resultado.
- Observações: libera COM com `Marshal.ReleaseComObject` em `finally`, porém chama `ReleaseComObject` mesmo se `doc`/`wordApp` puderem ser nulos — possível fonte de exceção. Recomendado checar `null` antes de `ReleaseComObject`.

Tagging helpers / Pipeline helpers (recap)
- TagManager, FilterAndTagPipelines, PipeMethods, HelperMethods, PipeUtils — formam o núcleo da lógica de análise de tubos, cálculo de inclinação e criação/exclusão de tags. Interagem fortemente com `ViewDirections` e parâmetros BuiltIn do Revit.

UI\ViewModels\LoginViewModel (já descrito) — responsável por habilitar a UI via `EnableUI()` que interage com `RibbonManager` e com `Transaction`.

Dev, outros comandos simples
- `Dev` é placeholder; `DiameterTag`, `SlopeTag`, `RealSlopeTag`, `FlowTag`, `SetAreasGP`, `SetAreasTR`, `RainNetwork`, `MemoHDS`, `SanFittings` são referenciados no UIBuilder — implementações nem todas mostradas, mas os nomes seguem o padrão `ProjetaHDR.Commands.<ClassName>`.

Classes/arquivos referenciados mas ausentes no snapshot
- AddinAppLoader — ponto de inicialização chamado em `AddinApp.OnStartup`.
- ResourceImage — helper que retorna `BitmapImage` dado um nome de arquivo (usado por RibbonManager e LoginViewModel). Necessário garantir que carrega imagens da pasta Resources corretamente.
- FixtureStorageManager — gerencia persistência de `AddedFixtureFamilies` no documento Revit.
- Implementações completas de `FixtureFamilyItem`, `AreaFamilyItem`, `InputFixtureItem`, `ObservableObject`, `RelayCommand`, `SaveDataStorageEvent` — assinaturas foram providas no snapshot; código completo não está presente.
- Outros comandos referenciados no UI (DiameterTag, SlopeTag, RealSlopeTag, FlowTag, SetAreasGP, SetAreasTR, SanFittings) — algumas implementações existem (`SanFittings` parcialmente), outras não foram entregues no snapshot.

Como rodar / Debug no Visual Studio
----------------------------------
1. Abrir solução no Visual Studio 2022 (target .NET Framework 4.8, C# 7.3).
2. Adicionar referências: `RevitAPI.dll` e `RevitAPIUI.dll` (Browse para C:\Program Files\Autodesk\Revit <year>\).
   - Em cada referência: setar Copy Local = false.
3. Configurar debug para iniciar o Revit:
   - Abra __Project Properties__ do projeto → aba __Debug__ → marque __Start external program__ e aponte para o executável do Revit (ex.: `C:\Program Files\Autodesk\Revit 2024\Revit.exe`).
   - Ou use o post-build script no `guia-iniciar-plugin.txt` para copiar .dll e .addin para `%AppData%\Autodesk\Revit\Addins\<Year>`.
4. Recursos (images) — coloque em pasta `Resources` no projeto, Build Action = __Resource__ e Copy to Output Directory = __Copy if newer__.
5. Para testar Word features garanta Microsoft Word instalado na máquina.
6. Para depurar UI/Windows WPF que se anexam ao Revit (DrenWindow, WordExportWindow), use Debug (F5) após configurar external program.

Observações importantes e boas práticas
--------------------------------------
- Transações: todo código que modifica documento Revit deve estar dentro de `Transaction`. Classes como `ExecuteDrenEvent` e `Seduc` seguem isso; revisar handlers para garantir Commit/rollback apropriados em caso de exceção.
- ExternalEvent: use `ExternalEvent` para executar operações que precisam do contexto do Revit a partir da thread UI WPF (já usado em DrenViewModel).
- COM Interop (Word): sempre liberar objetos COM com `Marshal.ReleaseComObject` e garantir `Quit()` do `Word.Application`. Implementar `Dispose` corretamente em `WordHandler`.
- Parâmetros: ao setar parâmetros por GUID ou LookupParameter, validar tipos (string/double) antes de setar e tratar `IsReadOnly`.
- Recursos e paths: usar `Assembly.GetExecutingAssembly().Location` para localizar `Resources` e `Docs` (como já feito em `DocHandler` e `WordExportViewModel`).
- Logging: adicionar logs mais robustos (arquivo/Trace) para facilitar debugging em clientes sem acesso ao console.

Resumo final e próximos passos sugeridos
---------------------------------------
- O código apresenta um design modular: UIBuilder/RibbonManager (UI), Commands (ações Revit), Services/Utils (lógica), ViewModels (WPF), Handlers (Word Interop).
- Falta adicionar/tornar explícitos alguns arquivos mencionados (ResourceImage, AddinAppLoader, FixtureStorageManager, implementações completas de alguns ViewModels/items). Recomendo:
  - Implementar/ajustar `Dispose` em `WordHandler`.
  - Proteger chamadas a `Marshal.ReleaseComObject` com checagem de `null`.
  - Tratar exceções por item ao setar parâmetros (Seduc / NestedFittings).
  - Adicionar logging estruturado.

Se quiser, eu gero um README pronto para o arquivo raiz substituindo o atual (já formatado em Markdown), ou produzo versões separadas (sumário + documentação por pasta). Também posso gerar sugestões de testes unitários para helpers (BasicUtils, PipeUtils) e um checklist de revisão de COM Interop. Qual opção prefere agora?
