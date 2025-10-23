namespace TaskWeb.Repositories;

using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class MateriaDatabaseRepository : DbConnection, IMateriaRepository
{
    public MateriaDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public void Create(Materia materia)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO Materia (Nome, CargaHorariaSemanal, TurmaId, ProfessorId)
            VALUES (@nome, @carga, @turmaId, @professorId)
            """;
        cmd.Parameters.AddWithValue("nome", materia.Nome);
        cmd.Parameters.AddWithValue("carga", materia.CargaHorariaSemanal);
        cmd.Parameters.AddWithValue("turmaId", materia.TurmaId);
        cmd.Parameters.AddWithValue("professorId", materia.ProfessorId);
        cmd.ExecuteNonQuery();
    }

    public Materia? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT m.MateriaId,
                   m.Nome,
                   m.CargaHorariaSemanal,
                   m.TurmaId,
                   m.ProfessorId,
                   t.Nome AS TurmaNome,
                   p.Nome AS ProfessorNome
            FROM Materia m
            JOIN Turma t ON m.TurmaId = t.TurmaId
            JOIN Professor p ON m.ProfessorId = p.ProfessorId
            WHERE m.MateriaId = @id
            """;
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapMateria(reader);
        }

        return null;
    }

    public List<Materia> ReadAll()
    {
        List<Materia> materias = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT m.MateriaId,
                   m.Nome,
                   m.CargaHorariaSemanal,
                   m.TurmaId,
                   m.ProfessorId,
                   t.Nome AS TurmaNome,
                   p.Nome AS ProfessorNome
            FROM Materia m
            JOIN Turma t ON m.TurmaId = t.TurmaId
            JOIN Professor p ON m.ProfessorId = p.ProfessorId
            ORDER BY t.Nome, m.Nome
            """;

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            materias.Add(MapMateria(reader));
        }

        return materias;
    }

    public List<Materia> ReadByTurma(int turmaId)
    {
        List<Materia> materias = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT m.MateriaId,
                   m.Nome,
                   m.CargaHorariaSemanal,
                   m.TurmaId,
                   m.ProfessorId,
                   t.Nome AS TurmaNome,
                   p.Nome AS ProfessorNome
            FROM Materia m
            JOIN Turma t ON m.TurmaId = t.TurmaId
            JOIN Professor p ON m.ProfessorId = p.ProfessorId
            WHERE m.TurmaId = @turmaId
            ORDER BY m.Nome
            """;
        cmd.Parameters.AddWithValue("turmaId", turmaId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            materias.Add(MapMateria(reader));
        }

        return materias;
    }

    private static Materia MapMateria(SqlDataReader reader)
    {
        return new Materia
        {
            MateriaId = (int)reader["MateriaId"],
            Nome = (string)reader["Nome"],
            CargaHorariaSemanal = (int)reader["CargaHorariaSemanal"],
            TurmaId = (int)reader["TurmaId"],
            ProfessorId = (int)reader["ProfessorId"],
            TurmaNome = (string)reader["TurmaNome"],
            ProfessorNome = (string)reader["ProfessorNome"]
        };
    }

    public void Update(Materia materia)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE Materia
            SET Nome = @nome,
                CargaHorariaSemanal = @carga,
                TurmaId = @turmaId,
                ProfessorId = @professorId
            WHERE MateriaId = @id
            """;
        cmd.Parameters.AddWithValue("nome", materia.Nome);
        cmd.Parameters.AddWithValue("carga", materia.CargaHorariaSemanal);
        cmd.Parameters.AddWithValue("turmaId", materia.TurmaId);
        cmd.Parameters.AddWithValue("professorId", materia.ProfessorId);
        cmd.Parameters.AddWithValue("id", materia.MateriaId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Materia WHERE MateriaId = @id";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}
