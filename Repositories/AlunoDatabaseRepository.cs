namespace TaskWeb.Repositories;

using System;
using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class AlunoDatabaseRepository : DbConnection, IAlunoRepository
{
    public AlunoDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public void Create(Aluno aluno)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO Aluno (Nome, Matricula, TurmaId)
            VALUES (@nome, @matricula, @turmaId)
            """;
        cmd.Parameters.AddWithValue("nome", aluno.Nome);
        cmd.Parameters.AddWithValue("matricula", aluno.Matricula);
        cmd.Parameters.AddWithValue("turmaId", aluno.TurmaId);

        cmd.ExecuteNonQuery();
    }

    public List<Aluno> ReadAll()
    {
        List<Aluno> alunos = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT a.AlunoId,
                   a.Nome,
                   a.Matricula,
                   a.TurmaId,
                   t.Nome AS TurmaNome
            FROM Aluno a
            JOIN Turma t ON a.TurmaId = t.TurmaId
            ORDER BY a.Nome
            """;

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            alunos.Add(new Aluno
            {
                AlunoId = (int)reader["AlunoId"],
                Nome = (string)reader["Nome"],
                Matricula = (string)reader["Matricula"],
                TurmaId = (int)reader["TurmaId"],
                TurmaNome = (string)reader["TurmaNome"]
            });
        }

        return alunos;
    }

    public Aluno? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT a.AlunoId,
                   a.Nome,
                   a.Matricula,
                   a.TurmaId,
                   t.Nome AS TurmaNome
            FROM Aluno a
            JOIN Turma t ON a.TurmaId = t.TurmaId
            WHERE a.AlunoId = @id
            """;
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Aluno
            {
                AlunoId = (int)reader["AlunoId"],
                Nome = (string)reader["Nome"],
                Matricula = (string)reader["Matricula"],
                TurmaId = (int)reader["TurmaId"],
                TurmaNome = (string)reader["TurmaNome"]
            };
        }

        return null;
    }

    public void Update(Aluno aluno)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE Aluno
            SET Nome = @nome,
                Matricula = @matricula,
                TurmaId = @turmaId
            WHERE AlunoId = @id
            """;
        cmd.Parameters.AddWithValue("nome", aluno.Nome);
        cmd.Parameters.AddWithValue("matricula", aluno.Matricula);
        cmd.Parameters.AddWithValue("turmaId", aluno.TurmaId);
        cmd.Parameters.AddWithValue("id", aluno.AlunoId);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Aluno WHERE AlunoId = @id";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}
