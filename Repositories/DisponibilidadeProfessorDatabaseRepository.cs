namespace TaskWeb.Repositories;

using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class DisponibilidadeProfessorDatabaseRepository : DbConnection, IDisponibilidadeProfessorRepository
{
    public DisponibilidadeProfessorDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public void Create(DisponibilidadeProfessor disponibilidade)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO DisponibilidadeProfessor (ProfessorId, DiaSemana, SlotAulaId)
            VALUES (@professorId, @diaSemana, @slotAulaId)
            """;
        cmd.Parameters.AddWithValue("professorId", disponibilidade.ProfessorId);
        cmd.Parameters.AddWithValue("diaSemana", disponibilidade.DiaSemana);
        cmd.Parameters.AddWithValue("slotAulaId", disponibilidade.SlotAulaId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand("DELETE FROM DisponibilidadeProfessor WHERE DisponibilidadeProfessorId = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    public void DeleteByProfessor(int professorId)
    {
        using SqlCommand cmd = new SqlCommand("DELETE FROM DisponibilidadeProfessor WHERE ProfessorId = @professorId", conn);
        cmd.Parameters.AddWithValue("professorId", professorId);
        cmd.ExecuteNonQuery();
    }

    public List<DisponibilidadeProfessor> ReadAll()
    {
        return ExecuteList(BaseSelect() + " ORDER BY dp.ProfessorId, dp.DiaSemana, sa.Sequencia");
    }

    public List<DisponibilidadeProfessor> ReadByProfessor(int professorId)
    {
        return ExecuteList(
            BaseSelect() + " WHERE dp.ProfessorId = @professorId ORDER BY dp.DiaSemana, sa.Sequencia",
            new SqlParameter("professorId", professorId));
    }

    public List<DisponibilidadeProfessor> ReadByProfessorAndDia(int professorId, int diaSemana)
    {
        return ExecuteList(
            BaseSelect() + " WHERE dp.ProfessorId = @professorId AND dp.DiaSemana = @diaSemana ORDER BY sa.Sequencia",
            new SqlParameter("professorId", professorId),
            new SqlParameter("diaSemana", diaSemana));
    }

    private List<DisponibilidadeProfessor> ExecuteList(string sql, params SqlParameter[] parameters)
    {
        List<DisponibilidadeProfessor> disponiveis = new();

        using SqlCommand cmd = new SqlCommand(sql, conn);
        if (parameters?.Length > 0)
        {
            cmd.Parameters.AddRange(parameters);
        }

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            disponiveis.Add(Map(reader));
        }

        return disponiveis;
    }

    private static string BaseSelect()
    {
        return """
            SELECT dp.DisponibilidadeProfessorId,
                   dp.ProfessorId,
                   dp.DiaSemana,
                   dp.SlotAulaId,
                   sa.Sequencia AS SlotSequencia,
                   sa.HoraInicio,
                   sa.HoraFim,
                   p.Nome AS ProfessorNome
            FROM DisponibilidadeProfessor dp
            JOIN Professor p ON dp.ProfessorId = p.ProfessorId
            JOIN SlotAula sa ON dp.SlotAulaId = sa.SlotAulaId
            """;
    }

    private static DisponibilidadeProfessor Map(SqlDataReader reader)
    {
        return new DisponibilidadeProfessor
        {
            DisponibilidadeProfessorId = (int)reader["DisponibilidadeProfessorId"],
            ProfessorId = (int)reader["ProfessorId"],
            DiaSemana = (int)reader["DiaSemana"],
            SlotAulaId = (int)reader["SlotAulaId"],
            SlotSequencia = (int)reader["SlotSequencia"],
            HoraInicio = (TimeSpan)reader["HoraInicio"],
            HoraFim = (TimeSpan)reader["HoraFim"],
            ProfessorNome = (string)reader["ProfessorNome"]
        };
    }
}
