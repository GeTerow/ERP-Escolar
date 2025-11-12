namespace TaskWeb.Repositories;

using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class TurmaDatabaseRepository : DbConnection, ITurmaRepository
{
    public TurmaDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public void Create(Turma turma)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO Turma (Nome, AnoLetivo, TurnoId)
            VALUES (@nome, @anoLetivo, @turnoId)
            """;
        cmd.Parameters.AddWithValue("nome", turma.Nome);
        cmd.Parameters.AddWithValue("anoLetivo", turma.AnoLetivo);
        cmd.Parameters.AddWithValue("turnoId", turma.TurnoId);
        cmd.ExecuteNonQuery();
    }

    public Turma? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = BaseSelect() + " WHERE t.TurmaId = @id";
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapTurma(reader);
        }

        return null;
    }

    public List<Turma> ReadAll()
    {
        List<Turma> turmas = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = BaseSelect() + " ORDER BY t.Nome";

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            turmas.Add(MapTurma(reader));
        }

        return turmas;
    }

    public void Update(Turma turma)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE Turma
            SET Nome = @nome,
                AnoLetivo = @anoLetivo,
                TurnoId = @turnoId
            WHERE TurmaId = @id
            """;
        cmd.Parameters.AddWithValue("nome", turma.Nome);
        cmd.Parameters.AddWithValue("anoLetivo", turma.AnoLetivo);
        cmd.Parameters.AddWithValue("turnoId", turma.TurnoId);
        cmd.Parameters.AddWithValue("id", turma.TurmaId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Turma WHERE TurmaId = @id";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    private static string BaseSelect()
    {
        return """
            SELECT t.TurmaId,
                   t.Nome,
                   t.AnoLetivo,
                   t.TurnoId,
                   tr.Nome AS TurnoNome
            FROM Turma t
            JOIN Turno tr ON t.TurnoId = tr.TurnoId
            """;
    }

    private static Turma MapTurma(SqlDataReader reader)
    {
        return new Turma
        {
            TurmaId = (int)reader["TurmaId"],
            Nome = (string)reader["Nome"],
            AnoLetivo = (string)reader["AnoLetivo"],
            TurnoId = (int)reader["TurnoId"],
            TurnoNome = (string)reader["TurnoNome"]
        };
    }
}
