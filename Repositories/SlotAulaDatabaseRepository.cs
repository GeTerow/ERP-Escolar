namespace TaskWeb.Repositories;

using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class SlotAulaDatabaseRepository : DbConnection, ISlotAulaRepository
{
    public SlotAulaDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public void Create(SlotAula slot)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO SlotAula (TurnoId, Sequencia, HoraInicio, HoraFim, EhIntervalo)
            VALUES (@turnoId, @sequencia, @horaInicio, @horaFim, @ehIntervalo)
            """;
        cmd.Parameters.AddWithValue("turnoId", slot.TurnoId);
        cmd.Parameters.AddWithValue("sequencia", slot.Sequencia);
        cmd.Parameters.AddWithValue("horaInicio", slot.HoraInicio);
        cmd.Parameters.AddWithValue("horaFim", slot.HoraFim);
        cmd.Parameters.AddWithValue("ehIntervalo", slot.EhIntervalo);
        cmd.ExecuteNonQuery();
    }

    public SlotAula? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = BaseSelect() + " WHERE s.SlotAulaId = @id";
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return Map(reader);
        }

        return null;
    }

    public List<SlotAula> ReadAll()
    {
        return ExecuteList(BaseSelect() + " ORDER BY t.HoraInicio, s.Sequencia");
    }

    public List<SlotAula> ReadByTurno(int turnoId)
    {
        return ExecuteList(
            BaseSelect() + " WHERE s.TurnoId = @turnoId ORDER BY s.Sequencia",
            new SqlParameter("turnoId", turnoId));
    }

    public void Update(SlotAula slot)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE SlotAula
            SET TurnoId = @turnoId,
                Sequencia = @sequencia,
                HoraInicio = @horaInicio,
                HoraFim = @horaFim,
                EhIntervalo = @ehIntervalo
            WHERE SlotAulaId = @id
            """;
        cmd.Parameters.AddWithValue("turnoId", slot.TurnoId);
        cmd.Parameters.AddWithValue("sequencia", slot.Sequencia);
        cmd.Parameters.AddWithValue("horaInicio", slot.HoraInicio);
        cmd.Parameters.AddWithValue("horaFim", slot.HoraFim);
        cmd.Parameters.AddWithValue("ehIntervalo", slot.EhIntervalo);
        cmd.Parameters.AddWithValue("id", slot.SlotAulaId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM SlotAula WHERE SlotAulaId = @id";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    private List<SlotAula> ExecuteList(string sql, params SqlParameter[] parameters)
    {
        List<SlotAula> slots = new();

        using SqlCommand cmd = new SqlCommand(sql, conn);
        if (parameters?.Length > 0)
        {
            cmd.Parameters.AddRange(parameters);
        }

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            slots.Add(Map(reader));
        }

        return slots;
    }

    private static string BaseSelect()
    {
        return """
            SELECT s.SlotAulaId,
                   s.TurnoId,
                   s.Sequencia,
                   s.HoraInicio,
                   s.HoraFim,
                   s.EhIntervalo,
                   t.Nome AS TurnoNome
            FROM SlotAula s
            JOIN Turno t ON s.TurnoId = t.TurnoId
            """;
    }

    private static SlotAula Map(SqlDataReader reader)
    {
        return new SlotAula
        {
            SlotAulaId = (int)reader["SlotAulaId"],
            TurnoId = (int)reader["TurnoId"],
            Sequencia = (int)reader["Sequencia"],
            HoraInicio = (TimeSpan)reader["HoraInicio"],
            HoraFim = (TimeSpan)reader["HoraFim"],
            EhIntervalo = (bool)reader["EhIntervalo"],
            TurnoNome = (string)reader["TurnoNome"]
        };
    }
}
