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

CREATE TABLE Turno

(

    TurnoId INT IDENTITY PRIMARY KEY,

    Nome VARCHAR(100) NOT NULL,

    HoraInicio TIME NOT NULL,

    HoraFim TIME NOT NULL

)



INSERT INTO Turno (Nome, HoraInicio, HoraFim) VALUES ('Manha', '07:30', '12:30')

INSERT INTO Turno (Nome, HoraInicio, HoraFim) VALUES ('Tarde', '13:00', '18:00')



CREATE TABLE SlotAula

(

    SlotAulaId INT IDENTITY PRIMARY KEY,

    TurnoId INT NOT NULL,

    Sequencia INT NOT NULL,

    HoraInicio TIME NOT NULL,

    HoraFim TIME NOT NULL,

    EhIntervalo BIT NOT NULL DEFAULT 0,

    FOREIGN KEY (TurnoId) REFERENCES Turno (TurnoId)

)



INSERT INTO SlotAula (TurnoId, Sequencia, HoraInicio, HoraFim, EhIntervalo) VALUES

(1, 1, '07:30', '08:20', 0),

(1, 2, '08:20', '09:10', 0),

(1, 3, '09:10', '10:00', 0),

(1, 4, '10:00', '10:20', 1),

(1, 5, '10:20', '11:10', 0),

(1, 6, '11:10', '12:00', 0),

(1, 7, '12:00', '12:30', 0),

(2, 1, '13:00', '13:50', 0),

(2, 2, '13:50', '14:40', 0),

(2, 3, '14:40', '15:30', 0),

(2, 4, '15:30', '15:50', 1),

(2, 5, '15:50', '16:40', 0),

(2, 6, '16:40', '17:30', 0),

(2, 7, '17:30', '18:00', 0)



CREATE TABLE Turma

(

    TurmaId INT IDENTITY PRIMARY KEY,

    Nome VARCHAR(100) NOT NULL,

    AnoLetivo VARCHAR(20) NOT NULL,

    TurnoId INT NOT NULL,

    FOREIGN KEY (TurnoId) REFERENCES Turno (TurnoId)

)



INSERT INTO Turma (Nome, AnoLetivo, TurnoId) VALUES ('1 Ano A', '2025', 1)

INSERT INTO Turma (Nome, AnoLetivo, TurnoId) VALUES ('2 Ano B', '2025', 2)

CREATE TABLE Professor
(
    ProfessorId INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Telefone VARCHAR(20) NOT NULL
)

INSERT INTO Professor (Nome, Email, Telefone) VALUES ('Ana Souza', 'ana@escola.com', '(11)1111-1111')
INSERT INTO Professor (Nome, Email, Telefone) VALUES ('Carlos Lima', 'carlos@escola.com', '(11)2222-2222')

CREATE TABLE DisponibilidadeProfessor
(
    DisponibilidadeProfessorId INT IDENTITY PRIMARY KEY,
    ProfessorId INT NOT NULL,
    DiaSemana INT NOT NULL,
    SlotAulaId INT NOT NULL,
    FOREIGN KEY (ProfessorId) REFERENCES Professor (ProfessorId),
    FOREIGN KEY (SlotAulaId) REFERENCES SlotAula (SlotAulaId)
)

-- Professora Ana disponivel no turno da manha em todos os dias uteis
INSERT INTO DisponibilidadeProfessor (ProfessorId, DiaSemana, SlotAulaId)
SELECT 1, dias.DiaSemana, s.SlotAulaId
FROM (VALUES (1),(2),(3),(4),(5)) AS dias(DiaSemana)
JOIN SlotAula s ON s.TurnoId = 1 AND s.EhIntervalo = 0

-- Professor Carlos disponivel no turno da tarde em todos os dias uteis
INSERT INTO DisponibilidadeProfessor (ProfessorId, DiaSemana, SlotAulaId)
SELECT 2, dias.DiaSemana, s.SlotAulaId
FROM (VALUES (1),(2),(3),(4),(5)) AS dias(DiaSemana)
JOIN SlotAula s ON s.TurnoId = 2 AND s.EhIntervalo = 0

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

    SlotAulaId INT NOT NULL,

    DiaSemana INT NOT NULL, -- 1=Segunda ... 5=Sexta

    FOREIGN KEY (TurmaId) REFERENCES Turma (TurmaId),

    FOREIGN KEY (MateriaId) REFERENCES Materia (MateriaId),

    FOREIGN KEY (ProfessorId) REFERENCES Professor (ProfessorId),

    FOREIGN KEY (SlotAulaId) REFERENCES SlotAula (SlotAulaId)

)



-- Grade demonstrativa para apresentacao inicial

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (1, 1, 1, 1, 1)

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (1, 2, 2, 2, 1)

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (1, 1, 1, 3, 3)

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (1, 2, 2, 5, 4)



INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (2, 3, 1, 8, 2)

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (2, 4, 2, 9, 2)

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (2, 3, 1, 10, 4)

INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana) VALUES (2, 4, 2, 12, 5)

