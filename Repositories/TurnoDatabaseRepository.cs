namespace TaskWeb.Repositories;

using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class TurnoDatabaseRepository : DbConnection, ITurnoRepository
{
    public TurnoDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public void Create(Turno turno)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO Turno (Nome, HoraInicio, HoraFim)
            VALUES (@nome, @horaInicio, @horaFim)
            """;
        cmd.Parameters.AddWithValue("nome", turno.Nome);
        cmd.Parameters.AddWithValue("horaInicio", turno.HoraInicio);
        cmd.Parameters.AddWithValue("horaFim", turno.HoraFim);
        cmd.ExecuteNonQuery();
    }

    public Turno? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT TurnoId, Nome, HoraInicio, HoraFim
            FROM Turno
            WHERE TurnoId = @id
            """;
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return Map(reader);
        }

        return null;
    }

    public List<Turno> ReadAll()
    {
        List<Turno> turnos = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT TurnoId, Nome, HoraInicio, HoraFim
            FROM Turno
            ORDER BY HoraInicio
            """;

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            turnos.Add(Map(reader));
        }

        return turnos;
    }

    public void Update(Turno turno)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE Turno
            SET Nome = @nome,
                HoraInicio = @horaInicio,
                HoraFim = @horaFim
            WHERE TurnoId = @id
            """;
        cmd.Parameters.AddWithValue("nome", turno.Nome);
        cmd.Parameters.AddWithValue("horaInicio", turno.HoraInicio);
        cmd.Parameters.AddWithValue("horaFim", turno.HoraFim);
        cmd.Parameters.AddWithValue("id", turno.TurnoId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Turno WHERE TurnoId = @id";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    private static Turno Map(SqlDataReader reader)
    {
        return new Turno
        {
            TurnoId = (int)reader["TurnoId"],
            Nome = (string)reader["Nome"],
            HoraInicio = (TimeSpan)reader["HoraInicio"],
            HoraFim = (TimeSpan)reader["HoraFim"]
        };
    }
}
