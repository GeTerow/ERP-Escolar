namespace TaskWeb.Repositories;

using System;
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
            INSERT INTO Turma (Nome, AnoLetivo)
            VALUES (@nome, @anoLetivo)
            """;
        cmd.Parameters.AddWithValue("nome", turma.Nome);
        cmd.Parameters.AddWithValue("anoLetivo", (object?)turma.AnoLetivo ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public Turma? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Turma WHERE TurmaId = @id";
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Turma
            {
                TurmaId = (int)reader["TurmaId"],
                Nome = (string)reader["Nome"],
                AnoLetivo = (string)reader["AnoLetivo"]
            };
        }

        return null;
    }

    public List<Turma> ReadAll()
    {
        List<Turma> turmas = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT TurmaId, Nome, AnoLetivo
            FROM Turma
            ORDER BY Nome
            """;

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            turmas.Add(new Turma
            {
                TurmaId = (int)reader["TurmaId"],
                Nome = (string)reader["Nome"],
                AnoLetivo = (string)reader["AnoLetivo"]
            });
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
                AnoLetivo = @anoLetivo
            WHERE TurmaId = @id
            """;
        cmd.Parameters.AddWithValue("nome", turma.Nome);
        cmd.Parameters.AddWithValue("anoLetivo", turma.AnoLetivo);
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
}
