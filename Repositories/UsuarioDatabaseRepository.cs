namespace TaskWeb.Repositories;

using TaskWeb.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class UsuarioDatabaseRepository : DbConnection, IUsuarioRepository
{
    public UsuarioDatabaseRepository(string? strConn) : base(strConn)
    {
    }

    public Usuario? Login(LoginViewModel model)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Usuario WHERE Email = @email AND Senha = @senha";
        cmd.Parameters.AddWithValue("email", model.Email);
        cmd.Parameters.AddWithValue("senha", model.Senha);

        using SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return Map(reader);
        }

        return null;
    }

    public void Create(Usuario usuario)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO Usuario (Nome, Email, Senha)
            VALUES (@nome, @email, @senha)
            """;
        cmd.Parameters.AddWithValue("nome", usuario.Nome);
        cmd.Parameters.AddWithValue("email", usuario.Email);
        cmd.Parameters.AddWithValue("senha", usuario.Senha);
        cmd.ExecuteNonQuery();
    }

    public List<Usuario> ReadAll()
    {
        List<Usuario> usuarios = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT UsuarioId, Nome, Email, Senha FROM Usuario ORDER BY Nome";

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            usuarios.Add(Map(reader));
        }

        return usuarios;
    }

    public Usuario? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT UsuarioId, Nome, Email, Senha FROM Usuario WHERE UsuarioId = @id";
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return Map(reader);
        }

        return null;
    }

    public void Update(Usuario usuario)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE Usuario
            SET Nome = @nome,
                Email = @email,
                Senha = @senha
            WHERE UsuarioId = @id
            """;
        cmd.Parameters.AddWithValue("nome", usuario.Nome);
        cmd.Parameters.AddWithValue("email", usuario.Email);
        cmd.Parameters.AddWithValue("senha", usuario.Senha);
        cmd.Parameters.AddWithValue("id", usuario.UsuarioId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Usuario WHERE UsuarioId = @id";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    private static Usuario Map(SqlDataReader reader)
    {
        return new Usuario
        {
            UsuarioId = (int)reader["UsuarioId"],
            Email = (string)reader["Email"],
            Nome = (string)reader["Nome"],
            Senha = (string)reader["Senha"] 
        };
    }
}