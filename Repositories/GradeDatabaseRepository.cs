namespace TaskWeb.Repositories;

using Microsoft.Data.SqlClient;
using TaskWeb.Models;

public class GradeDatabaseRepository : DbConnection, IGradeRepository
{
    public GradeDatabaseRepository(string? connStr) : base(connStr)
    {
    }

    public List<GradeHorario> ReadByTurma(int turmaId)
    {
        string sql = BaseSelect() + " WHERE g.TurmaId = @turmaId ORDER BY sa.Sequencia, g.DiaSemana";
        return ExecuteList(sql, new SqlParameter("turmaId", turmaId));
    }

    public List<GradeHorario> ReadByProfessor(int professorId)
    {
        string sql = BaseSelect() + " WHERE g.ProfessorId = @professorId ORDER BY sa.Sequencia, g.DiaSemana";
        return ExecuteList(sql, new SqlParameter("professorId", professorId));
    }

    public GradeHorario? Read(int id)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = BaseSelect() + " WHERE g.GradeHorarioId = @id";
        cmd.Parameters.AddWithValue("id", id);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapGrade(reader);
        }

        return null;
    }

    public void Create(GradeHorario horario)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            INSERT INTO GradeHorario (TurmaId, MateriaId, ProfessorId, SlotAulaId, DiaSemana)
            VALUES (@turmaId, @materiaId, @professorId, @slotAulaId, @diaSemana)
            """;
        AddCommandParameters(cmd, horario);
        cmd.ExecuteNonQuery();
    }

    public void Update(GradeHorario horario)
    {
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            UPDATE GradeHorario
            SET TurmaId = @turmaId,
                MateriaId = @materiaId,
                ProfessorId = @professorId,
                SlotAulaId = @slotAulaId,
                DiaSemana = @diaSemana
            WHERE GradeHorarioId = @id
            """;
        AddCommandParameters(cmd, horario);
        cmd.Parameters.AddWithValue("id", horario.GradeHorarioId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlCommand cmd = new SqlCommand("DELETE FROM GradeHorario WHERE GradeHorarioId = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    public void DeleteByTurma(int turmaId)
    {
        using SqlCommand cmd = new SqlCommand("DELETE FROM GradeHorario WHERE TurmaId = @turmaId", conn);
        cmd.Parameters.AddWithValue("turmaId", turmaId);
        cmd.ExecuteNonQuery();
    }

    private List<GradeHorario> ExecuteList(string sql, params SqlParameter[] parameters)
    {
        List<GradeHorario> grade = new();

        using SqlCommand cmd = new SqlCommand(sql, conn);
        if (parameters?.Length > 0)
        {
            cmd.Parameters.AddRange(parameters);
        }

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            grade.Add(MapGrade(reader));
        }

        return grade;
    }

    private static void AddCommandParameters(SqlCommand cmd, GradeHorario horario)
    {
        cmd.Parameters.AddWithValue("turmaId", horario.TurmaId);
        cmd.Parameters.AddWithValue("materiaId", horario.MateriaId);
        cmd.Parameters.AddWithValue("professorId", horario.ProfessorId);
        cmd.Parameters.AddWithValue("slotAulaId", horario.SlotAulaId);
        cmd.Parameters.AddWithValue("diaSemana", horario.DiaSemana);
    }

    private static string BaseSelect()
    {
        return """
            SELECT g.GradeHorarioId,
                   g.TurmaId,
                   g.MateriaId,
                   g.ProfessorId,
                   g.SlotAulaId,
                   g.DiaSemana,
                   sa.Sequencia AS SlotSequencia,
                   sa.EhIntervalo AS SlotEhIntervalo,
                   sa.HoraInicio AS HoraInicio,
                   sa.HoraFim AS HoraFim,
                   t.Nome AS TurmaNome,
                   m.Nome AS MateriaNome,
                   p.Nome AS ProfessorNome
            FROM GradeHorario g
            JOIN Turma t ON g.TurmaId = t.TurmaId
            JOIN Materia m ON g.MateriaId = m.MateriaId
            JOIN Professor p ON g.ProfessorId = p.ProfessorId
            JOIN SlotAula sa ON g.SlotAulaId = sa.SlotAulaId
            """;
    }

    private static GradeHorario MapGrade(SqlDataReader reader)
    {
        return new GradeHorario
        {
            GradeHorarioId = (int)reader["GradeHorarioId"],
            TurmaId = (int)reader["TurmaId"],
            MateriaId = (int)reader["MateriaId"],
            ProfessorId = (int)reader["ProfessorId"],
            SlotAulaId = (int)reader["SlotAulaId"],
            DiaSemana = (int)reader["DiaSemana"],
            SlotSequencia = (int)reader["SlotSequencia"],
            SlotEhIntervalo = (bool)reader["SlotEhIntervalo"],
            HoraInicio = (TimeSpan)reader["HoraInicio"],
            HoraFim = (TimeSpan)reader["HoraFim"],
            TurmaNome = (string)reader["TurmaNome"],
            MateriaNome = (string)reader["MateriaNome"],
            ProfessorNome = (string)reader["ProfessorNome"]
        };
    }
}
