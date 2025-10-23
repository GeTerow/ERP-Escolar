# Documentacao Completa do Sistema de Grade Escolar (MVP)

Este documento descreve em detalhes cada componente do MVP desenvolvido para gerenciamento de grade escolar. A proposta foi implementar um sistema simples, baseado em ASP.NET Core MVC com Razor Views e persistencia em SQL Server, seguindo o roteiro apresentado em `Sistema_Grade_Escolar_MVP.md`. A arquitetura escolhida preserva a autenticacao basica por usuario e reorganiza o projeto original (de tarefas) para o dominio escolar.

## Visao Geral do Fluxo

1. O usuario acessa o sistema pela rota `/usuario/login`, autentica-se e e redirecionado para a Home (`/home`), que funciona como painel de navegacao.
2. A partir da Home, e possivel navegar para as areas de cadastro/listagem de Turmas, Professores, Materias, Alunos e para a tela da Grade.
3. Cada tela de cadastro oferece CRUD completo: listar, criar, editar e excluir registros, exibindo mensagens de sucesso ou erro via `TempData`.
4. A tela de Grade apresenta uma grade demonstrativa, com dados pre-carregados apenas para visualizacao nesta primeira entrega.

## Estrutura de Pastas

- `Controllers/`: contem os controllers MVC responsaveis por receber requisicoes HTTP, acionar repositorios e construir respostas (Views ou redirects).
- `Models/`: armazena classes de dominio e view models usados nas views e controllers.
- `Repositories/`: contem as classes de acesso a dados (ADO.NET) e interfaces que abstraem as operacoes.
- `Views/`: abriga as Razor Views, organizadas em subpastas por controller.
- `DOCUMENTACAO_SISTEMA_GRADE.md`: este arquivo, com toda a documentacao detalhada.
- `Sistema_Grade_Escolar_MVP.md`: roteiro original do professor, preservado para referencia.
- `Program.cs`, `appsettings*.json`: bootstrapping da aplicacao e configuracoes.
- `script.sql`: script SQL para provisionar o banco `BDGradeEscolar`.

## Configuracao da Aplicacao

### Program.cs

Arquivo: `Program.cs`
Responsabilidades:
- Cria o builder WebApplication.
- Registra os repositorios concretos como servicos `Transient`, cada um recebendo a connection string `default` do `appsettings.Development.json`.
- Ativa `Session` para armazenar dados do usuario logado (`UsuarioId`, `Nome`).
- Configura MVC (`AddControllersWithViews`) e o pipeline HTTP basico.
- Define a rota padrao para `UsuarioController.Login`, garantindo que usuarios nao autenticados cheguem ao formulario de login.
- Executa `app.Run()`.

### Configuracao de Banco

Arquivo: `appsettings.Development.json`
Contem a connection string apontando para o banco `BDGradeEscolar`. A string assume:
- SQL Server local (`Server=localhost`).
- Banco `BDGradeEscolar` (criado por `script.sql`).
- Autenticacao SQL (`User Id=aluno`, `Password=dba`, `Integrated Security=False`).

### Script de Banco

Arquivo: `script.sql`
Realiza as seguintes operacoes:
1. Cria o banco `BDGradeEscolar`.
2. Cria e popula a tabela `Usuario` com um usuario padrao (Coordenador).
3. Cria tabelas para `Turma`, `Professor`, `Materia`, `Aluno` e `GradeHorario`, com relacionamentos e algumas insercoes de exemplo.
4. Insere uma grade demonstrativa em `GradeHorario` para ser utilizada na apresentacao inicial.
5. Todas as tabelas seguem indices identity e constraints de chave estrangeira coerentes com o modelo.

## Models (Dominio e View Models)

### Usuario
Arquivo: `Models/Usuario.cs`
Campos: `UsuarioId`, `Nome`, `Email`, `Senha`.
Utilizacao: retorna dados de autenticacao e identifica o usuario conectado.

### LoginViewModel
Arquivo: `Models/LoginViewModel.cs`
Campos: `Email`, `Senha`.
Utilizacao: view model passado entre o formulario de login e o controller.

### Turma
Arquivo: `Models/Turma.cs`
Campos: `TurmaId`, `Nome`, `AnoLetivo`.
Utilizacao: representa turmas cadastradas; `AnoLetivo` e opcional, permitindo valores nulos.

### Professor
Arquivo: `Models/Professor.cs`
Campos: `ProfessorId`, `Nome`, `Email`, `Telefone`.
Utilizacao: professor disponivel para lecionar; email e telefone sao opcionais.

### Materia
Arquivo: `Models/Materia.cs`
Campos principais: `MateriaId`, `Nome`, `CargaHorariaSemanal`, `TurmaId`, `ProfessorId`.
Campos auxiliares (apenas exibicao): `TurmaNome`, `ProfessorNome`.
Utilizacao: define as disciplinas da turma, associando cada materia a um professor e a uma carga horaria semanal de aulas.

### Aluno
Arquivo: `Models/Aluno.cs`
Campos: `AlunoId`, `Nome`, `Matricula`, `DataNascimento`, `TurmaId`, `TurmaNome`.
Notas: `TurmaNome` e atributo auxiliar para facilitar exibicao nas views.

### GradeHorario
Arquivo: `Models/GradeHorario.cs`
Campos: `GradeHorarioId`, `TurmaId`, `MateriaId`, `ProfessorId`, `DiaSemana`, `HoraInicio`, `HoraFim`, `TurmaNome`, `MateriaNome`, `ProfessorNome`.
Utilizacao: representa cada aula agendada na grade final; os campos de nome sao usados para exibir informacoes legiveis na tela da grade.

### GradeViewModel
Arquivo: `Models/GradeViewModel.cs`
Estrutura composta:
- `Turmas`: lista de turmas disponiveis para selecao na view.
- `TurmaSelecionadaId`: turma ativa na exibicao.
- `Linhas`: lista de `GradeLinhaViewModel`.

`GradeLinhaViewModel` inclui:
- `Hora`: horario base da linha (TimeSpan).
- `AulasPorDia`: dicionario `DiaSemana -> GradeHorario?` representando a aula agendada para cada dia naquele horario.

## Repositories (Persistencia ADO.NET)

Todos os repositorios herdam de `DbConnection`, classe abstrata (Arquivo: `Repositories/DbConnection.cs`), que abre e fecha a conexao SQL no construtor e no `Dispose`. Todos utilizam `SqlCommand`, `SqlDataReader` e comandos parametrizados.

### IUsuarioRepository / UsuarioDatabaseRepository
Arquivos: `Repositories/IUsuarioRepository.cs`, `Repositories/UsuarioDatabaseRepository.cs`
Metodos:
- `Usuario? Login(LoginViewModel model)`.

Implementacao:
- Executa SELECT usando email e senha.
- Retorna instancia de `Usuario` se encontrar correspondencia ou `null` caso contrario.

### ITurmaRepository / TurmaDatabaseRepository
Arquivos: `Repositories/ITurmaRepository.cs`, `Repositories/TurmaDatabaseRepository.cs`
Metodos:
- `List<Turma> ReadAll()`.
- `Turma? Read(int id)`.
- `void Create(Turma turma)`.
- `void Update(Turma turma)`.
- `void Delete(int id)`.

Implementacao:
- `Create` e `Update`: manipulam `Nome` e `AnoLetivo`, usando `DBNull` quando o ano letivo e nulo.
- `ReadAll`: SELECT ordenado por nome.
- `Read`: busca individual por `TurmaId`.
- `Delete`: remove registro pela chave primaria.

### IProfessorRepository / ProfessorDatabaseRepository
Arquivos: `Repositories/IProfessorRepository.cs`, `Repositories/ProfessorDatabaseRepository.cs`
Metodos:
- `List<Professor> ReadAll()`.
- `Professor? Read(int id)`.
- `void Create(Professor professor)`.
- `void Update(Professor professor)`.
- `void Delete(int id)`.

Detalhes:
- Trata campos opcionais (Email, Telefone) com `DBNull.Value` nos metodos de insert/update.
- `Delete` executa remocao direta por `ProfessorId`.
- Ordena professores por nome para exibicao consistente.

### IMateriaRepository / MateriaDatabaseRepository
Arquivos: `Repositories/IMateriaRepository.cs`, `Repositories/MateriaDatabaseRepository.cs`
Metodos:
- `List<Materia> ReadAll()`.
- `List<Materia> ReadByTurma(int turmaId)`.
- `Materia? Read(int id)`.
- `void Create(Materia materia)`.
- `void Update(Materia materia)`.
- `void Delete(int id)`.

Implementacao:
- `Create` e `Update`: manipulam `TurmaId`, `ProfessorId` e `CargaHorariaSemanal`, garantindo integridade referencial.
- Selects usam JOIN com `Turma` e `Professor` para preencher campos de exibicao (`TurmaNome`, `ProfessorNome`).

### IAlunoRepository / AlunoDatabaseRepository
Arquivos: `Repositories/IAlunoRepository.cs`, `Repositories/AlunoDatabaseRepository.cs`
Metodos:
- `List<Aluno> ReadAll()`.
- `Aluno? Read(int id)`.
- `void Create(Aluno aluno)`.
- `void Update(Aluno aluno)`.
- `void Delete(int id)`.

Detalhes:
- `Create` e `Update`: tratam `DataNascimento` opcional usando `DBNull.Value` quando nula.
- `ReadAll` e `Read` executam JOIN com `Turma` para retornar `TurmaNome`.
- `Delete` remove o aluno pelo identificador.

### IGradeRepository / GradeDatabaseRepository
Arquivos: `Repositories/IGradeRepository.cs`, `Repositories/GradeDatabaseRepository.cs`
Metodos:
- `List<GradeHorario> ReadByTurma(int turmaId)`.

Detalhes:
- O repositorio apenas realiza leitura; os dados demonstrativos da grade sao inseridos pelo script SQL inicial.

## Controllers e Rotas

Todos os controllers seguem padrao:
1. Verificam se o usuario esta logado via `HttpContext.Session.GetInt32("UsuarioId")`. Caso contrario, redirecionam para `/usuario/login` (exceto o proprio `UsuarioController`).
2. Interagem com os repositorios apropriados para cada entidade.
3. Retornam Views com modelos ou redirect para a acao Index correspondente.
4. Utilizam `TempData` para transportar mensagens de sucesso/erro apos operacoes POST.

### UsuarioController
Arquivo: `Controllers/UsuarioController.cs`
Rotas principais:
- `GET /usuario/login` -> `Login()` exibe formulario.
- `POST /usuario/login` -> `Login(LoginViewModel)` processa credenciais.
- `GET /usuario/logout` -> `Logout()` encerra a sessao.

Detalhes:
- Utiliza `ViewBag.Error` para relatar falhas de autenticacao.
- Em caso de sucesso armazena `UsuarioId` e `Nome` na sessao e redireciona para `/home`.

### HomeController
Arquivo: `Controllers/HomeController.cs`
Rotas:
- `GET /home` -> `Index()` verifica sessao e retorna a view com cards de navegacao.

View associada: `Views/Home/Index.cshtml` apresenta atalhos para todos os modulos e exibe saudacao com nome do usuario, alem de botao de logout.

### TurmasController
Arquivo: `Controllers/TurmasController.cs`
Rotas principais:
- `GET /turmas` -> `Index()` lista turmas cadastradas, exibe alertas de `TempData` e disponibiliza botoes de editar/excluir.
- `GET /turmas/create` -> `Create()` exibe formulario vazio.
- `POST /turmas/create` -> `Create(Turma)` valida nome e grava nova turma, retornando mensagem de sucesso.
- `GET /turmas/edit/{id}` -> `Edit(int id)` carrega dados existentes ou retorna 404.
- `POST /turmas/edit` -> `Edit(Turma)` valida e atualiza registro, tratando excecoes de banco com mensagem amigavel.
- `GET /turmas/delete/{id}` -> `Delete(int id)` solicita confirmacao.
- `POST /turmas/delete` -> `DeleteConfirmed(int id)` remove turma quando possivel e informa caso haja vinculos impeditivos.

Views:
- `Views/Turmas/Index.cshtml`: tabela responsiva com coluna de acoes e alertas de `TempData`.
- `Views/Turmas/Create.cshtml` e `Views/Turmas/Edit.cshtml`: formularios identicos (edicao inclui campo oculto).
- `Views/Turmas/Delete.cshtml`: tela de confirmacao exibindo dados basicos.

### ProfessoresController
Arquivo: `Controllers/ProfessoresController.cs`
Rotas:
- `GET /professores` -> `Index()` lista professores com botoes de acao e alertas.
- `GET /professores/create` / `POST /professores/create` -> cadastro com validacao de nome.
- `GET /professores/edit/{id}` / `POST /professores/edit` -> edicao com tratamento de excecoes.
- `GET /professores/delete/{id}` / `POST /professores/delete` -> confirmacao e exclusao com mensagens adequadas.

Views:
- `Views/Professores/Index.cshtml`: tabela com email/telefone (ou "-") e acoes.
- `Views/Professores/Create.cshtml` e `Views/Professores/Edit.cshtml`: formularios compartilhando layout.
- `Views/Professores/Delete.cshtml`: confirmacao destacando nome, email e telefone.

### MateriasController
Arquivo: `Controllers/MateriasController.cs`
Rotas:
- `GET /materias` -> `Index()` mostra lista completa com alertas de CRUD.
- `GET /materias/create` / `POST /materias/create` -> cadastro validando nome e carga horaria (>0) e populando selects.
- `GET /materias/edit/{id}` / `POST /materias/edit` -> atualiza dados mantendo listas de turmas/professores em caso de erro.
- `GET /materias/delete/{id}` / `POST /materias/delete` -> confirmacao e remocao com notificacao de vinculos.

Views:
- `Views/Materias/Index.cshtml`: tabela com dados relacionados e botoes de acao.
- `Views/Materias/Create.cshtml` e `Views/Materias/Edit.cshtml`: formularios com selects preenchidos via `ViewBag`.
- `Views/Materias/Delete.cshtml`: tela de confirmacao apresentando carga horaria, turma e professor.

### AlunosController
Arquivo: `Controllers/AlunosController.cs`
Rotas:
- `GET /alunos` -> `Index()` lista alunos, exibe `TempData` e disponibiliza botoes de editar/excluir.
- `GET /alunos/create` / `POST /alunos/create` -> cadastro com validacao de Nome e Matricula e carga da lista de turmas.
- `GET /alunos/edit/{id}` / `POST /alunos/edit` -> edicao reaproveitando a mesma validacao e repovoando turmas em caso de erro.
- `GET /alunos/delete/{id}` / `POST /alunos/delete` -> confirmacao e exclusao com tratamento de erros de integridade.

Views:
- `Views/Alunos/Index.cshtml`: tabela com dados principais, data formatada e coluna de acoes.
- `Views/Alunos/Create.cshtml` e `Views/Alunos/Edit.cshtml`: formularios com campos de texto/data/select.
- `Views/Alunos/Delete.cshtml`: confirmacao mostrando matricula, data e turma.

### GradeController
Arquivo: `Controllers/GradeController.cs`
Rotas:
- `GET /grade` (parametro opcional `turmaId`) -> exibe a grade demonstrativa da turma selecionada.

#### Detalhes do GET /grade
1. Verifica a sessao e redireciona para `/usuario/login` se necessario.
2. Carrega todas as turmas; se a lista estiver vazia, retorna a view com modelo em branco.
3. Determina a turma ativa a partir do parametro `turmaId` ou escolhe a primeira turma cadastrada.
4. Consulta o repositorio de grade para obter os horarios previamente cadastrados pelo script SQL.
5. Gera uma lista de horarios distintos; se nao houver dados, preenche com horarios padrao (08h ate 14h) e sinaliza que se trata de uma demonstracao.
6. Monta `GradeViewModel` preenchendo o dicionario `dia -> aula` a partir do resultado.
7. Retorna a view `Views/Grade/Index.cshtml`, que apresenta apenas a visualizacao da grade (sem acao de geracao automatica nesta etapa).

## Views (Razor)

Todas as views utilizam Bootstrap 5 via CDN para manter consistencia visual e simplicidade. Cada view e focada em listar registros ou oferecer formulario de cadastro conforme escopo do MVP.

### Views/Usuario/Login.cshtml
- Formulario de login com campos `Email` e `Senha`.
- Exibe mensagens de erro quando `ViewBag.Error` possui valor.
- Submete para `/usuario/login` e utiliza classes Bootstrap para organizacao em grid.

### Views/Home/Index.cshtml
- Painel com cards que levam a cada modulo.
- Mostra saudacao com nome armazenado na sessao e botao de logout.
- Todos os cards utilizam componente `card` do Bootstrap, mantendo layout uniforme.

### Views/Turmas
- `Index.cshtml`: tabela responsiva com coluna de acoes (editar/excluir) e alertas de `TempData`.
- `Create.cshtml` / `Edit.cshtml`: formularios para cadastro e edicao com campos Nome e AnoLetivo; compartilham layout.
- `Delete.cshtml`: confirmacao com resumo da turma e botoes de confirmacao/cancelamento.

### Views/Professores
- `Index.cshtml`: lista professores com coluna de acoes e exibicao de email/telefone ou "-" quando ausente.
- `Create.cshtml` / `Edit.cshtml`: formularios similares com campos Nome, Email e Telefone.
- `Delete.cshtml`: confirmacao destacando dados principais.

### Views/Materias
- `Index.cshtml`: apresenta Materia, Carga Horaria, Turma e Professor em tabela com acoes.
- `Create.cshtml` / `Edit.cshtml`: formularios com selects dinamicos de turma e professor e validacao de carga horaria.
- `Delete.cshtml`: confirmacao exibindo informacoes de relacionamento para avaliar impacto.

### Views/Alunos
- `Index.cshtml`: tabela com Nome, Matricula, Data formatada, Turma e botoes de CRUD.
- `Create.cshtml` / `Edit.cshtml`: inputs para Nome, Matricula, Data (opcional) e select de Turma, reutilizando layout.
- `Delete.cshtml`: confirmacao com resumo do aluno e mensagem de atencao.

### Views/Grade
- `Index.cshtml`: inclui select de turma com auto submit e uma faixa informativa destacando que a grade e demonstrativa.
- Mostra somente a tabela com colunas de segunda a sexta; os dados exibidos sao os cadastrados previamente no banco.
- Cada celula mostra Materia e Professor quando ha aula; caso contrario, exibe "-" em texto acinzentado.

## Comportamento de Login e Sessao
- `UsuarioController` grava `UsuarioId` e `Nome` na sessao ao autenticar.
- Controllers restantes validam a presenca desse identificador antes de executar qualquer logica.
- `app.UseSession()` e `builder.Services.AddSession()` estao configurados em `Program.cs`.
- `/usuario/logout` limpa a sessao com `HttpContext.Session.Clear()`.

## Controle de Erros e Validacao
- Todos os controllers validam sessao antes de executar qualquer acao (exceto `UsuarioController`).
- `AlunosController` exige Nome e Matricula, repopula a lista de turmas e exibe `ViewBag.Error` quando invalido.
- `ProfessoresController` exige Nome e trata excecoes de banco, retornando mensagem amigavel na view.
- `MateriasController` valida Nome e `CargaHorariaSemanal > 0`, mantendo os selects preenchidos em caso de erro.
- `TurmasController` garante Nome preenchido e avisa quando a exclusao falha por vinculacoes.
- `GradeController` apenas apresenta mensagem informativa quando a grade exibida e demonstrativa.
- Mensagens de sucesso/erro persistem entre redirects via `TempData["Success"]` e `TempData["Error"]`.

## Como Executar o MVP
1. Execute `script.sql` em uma instancia do SQL Server para criar o banco `BDGradeEscolar`.
2. Ajuste a connection string em `appsettings.Development.json` se necessario.
3. Rode `dotnet build` para restaurar pacotes e compilar (comando ja validado).
4. Execute `dotnet run` para iniciar a aplicacao localmente.
5. Acesse `/usuario/login` e utilize as credenciais iniciais `coordenador@escola.com` / `123`.
6. Cadastre dados adicionais (turmas, professores, materias e alunos) e utilize a tela de grade apenas para apresentar a grade demonstrativa desta etapa.


## Resumo Final
O projeto foi reestruturado para o dominio escolar mantendo simplicidade e clareza. A atual versao oferece CRUD completo para turmas, professores, materias e alunos, com feedback ao usuario e validacoes basicas, alem de exibir uma grade semanal demonstrativa pre-carregada para a primeira apresentacao. O documento descreve controllers, models, repositorios, views, fluxo de login e passos de execucao, servindo como guia completo para a exibicao do MVP.
