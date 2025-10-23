CREATE DATABASE BDGradeEscolar
GO

USE BDGradeEscolar
GO

CREATE TABLE Usuario
(
    UsuarioId INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Senha VARCHAR(100) NOT NULL
)

INSERT INTO Usuario (Nome, Email, Senha) VALUES ('Coordenador', 'coordenador@escola.com', '123')

CREATE TABLE Turma
(
    TurmaId INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    AnoLetivo VARCHAR(20) NOT NULL
)

INSERT INTO Turma (Nome, AnoLetivo) VALUES ('1 Ano A', '2025')
INSERT INTO Turma (Nome, AnoLetivo) VALUES ('2 Ano B', '2025')

CREATE TABLE Professor
(
    ProfessorId INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Telefone VARCHAR(20) NOT NULL
)

INSERT INTO Professor (Nome, Email, Telefone) VALUES ('Ana Souza', 'ana@escola.com', '(11)1111-1111')
INSERT INTO Professor (Nome, Email, Telefone) VALUES ('Carlos Lima', 'carlos@escola.com', '(11)2222-2222')

CREATE TABLE Materia
(
    MateriaId INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    CargaHorariaSemanal INT NOT NULL,
    TurmaId INT NOT NULL,
    ProfessorId INT NOT NULL,
    FOREIGN KEY (TurmaId) REFERENCES Turma (TurmaId),
    FOREIGN KEY (ProfessorId) REFERENCES Professor (ProfessorId)
)

INSERT INTO Materia (Nome, CargaHorariaSemanal, TurmaId, ProfessorId) VALUES ('Matematica', 4, 1, 1)
INSERT INTO Materia (Nome, CargaHorariaSemanal, TurmaId, ProfessorId) VALUES ('Historia', 3, 1, 2)
INSERT INTO Materia (Nome, CargaHorariaSemanal, TurmaId, ProfessorId) VALUES ('Fisica', 3, 2, 1)
INSERT INTO Materia (Nome, CargaHorariaSemanal, TurmaId, ProfessorId) VALUES ('Quimica', 2, 2, 2)

CREATE TABLE Aluno
(
    AlunoId INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(120) NOT NULL,
    Matricula VARCHAR(30) NOT NULL UNIQUE,
    TurmaId INT NOT NULL,
    FOREIGN KEY (TurmaId) REFERENCES Turma (TurmaId)
)

INSERT INTO Aluno (Nome, Matricula, TurmaId) VALUES ('Bruno Santos', '2025001', 1)
INSERT INTO Aluno (Nome, Matricula, TurmaId) VALUES ('Maria Alves', '2025002', 1)
INSERT INTO Aluno (Nome, Matricula, TurmaId) VALUES ('Lucas Pereira', '2026001', 2)

CREATE TABLE GradeHorario
(
    GradeHorarioId INT IDENTITY PRIMARY KEY,
    TurmaId INT NOT NULL,
    MateriaId INT NOT NULL,
    ProfessorId INT NOT NULL,
    DiaSemana INT NOT NULL, -- 1=Segunda ... 5=Sexta
    HoraInicio TIME NOT NULL,
    HoraFim TIME NOT NULL,
    FOREIGN KEY (TurmaId) REFERENCES Turma (TurmaId),
    FOREIGN KEY (MateriaId) REFERENCES Materia (MateriaId),
    FOREIGN KEY (ProfessorId) REFERENCES Professor (ProfessorId)
)

-- Grade demonstrativa para apresentacao inicial
INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (1, 1, 1, 1, '08:00', '09:00')
INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (1, 2, 2, 1, '09:00', '10:00')
INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (1, 1, 1, 3, '08:00', '09:00')
INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (1, 2, 2, 4, '10:00', '11:00')

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (2, 3, 1, 2, '08:00', '09:00')
INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (2, 4, 2, 2, '09:00', '10:00')
INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (2, 3, 1, 4, '08:00', '09:00')
INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, DiaSemana, HoraInicio, HoraFim) VALUES (2, 4, 2, 5, '09:00', '10:00')
