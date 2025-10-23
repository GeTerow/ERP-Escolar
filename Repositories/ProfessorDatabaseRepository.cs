namespace TaskWeb.Repositories;

using System;
using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class ProfessorDatabaseRepository : DbConnection, IProfessorRepository
{
    public ProfessorDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public void Create(Professor professor)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO Professor (Nome, Email, Telefone)
            VALUES (@nome, @email, @telefone)
            """;
        cmd.Parameters.AddWithValue("nome", professor.Nome);
        cmd.Parameters.AddWithValue("email", professor.Email);
        cmd.Parameters.AddWithValue("telefone", professor.Telefone);
        cmd.ExecuteNonQuery();
    }

    public Professor? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Professor WHERE ProfessorId = @id";
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Professor
            {
                ProfessorId = (int)reader["ProfessorId"],
                Nome = (string)reader["Nome"],
                Email = (string)reader["Email"],
                Telefone = (string)reader["Telefone"]
            };
        }

        return null;
    }

    public List<Professor> ReadAll()
    {
        List<Professor> professores = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT ProfessorId, Nome, Email, Telefone
            FROM Professor
            ORDER BY Nome
            """;

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            professores.Add(new Professor
            {
                ProfessorId = (int)reader["ProfessorId"],
                Nome = (string)reader["Nome"],
                Email = (string)reader["Email"],
                Telefone = (string)reader["Telefone"]
            });
        }

        return professores;
    }

    public void Update(Professor professor)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE Professor
            SET Nome = @nome,
                Email = @email,
                Telefone = @telefone
            WHERE ProfessorId = @id
            """;
        cmd.Parameters.AddWithValue("nome", professor.Nome);
        cmd.Parameters.AddWithValue("email", professor.Email);
        cmd.Parameters.AddWithValue("telefone", professor.Telefone);
        cmd.Parameters.AddWithValue("id", professor.ProfessorId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Professor WHERE ProfessorId = @id";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}
