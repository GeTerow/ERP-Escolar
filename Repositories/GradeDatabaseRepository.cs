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
        List<GradeHorario> grade = new();

        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = """
            SELECT g.GradeHorarioId,
                   g.TurmaId,
                   g.MateriaId,
                   g.ProfessorId,
                   g.DiaSemana,
                   g.HoraInicio,
                   g.HoraFim,
                   t.Nome AS TurmaNome,
                   m.Nome AS MateriaNome,
                   p.Nome AS ProfessorNome
            FROM GradeHorario g
            JOIN Turma t ON g.TurmaId = t.TurmaId
            JOIN Materia m ON g.MateriaId = m.MateriaId
            JOIN Professor p ON g.ProfessorId = p.ProfessorId
            WHERE g.TurmaId = @turmaId
            ORDER BY g.HoraInicio, g.DiaSemana
            """;
        cmd.Parameters.AddWithValue("turmaId", turmaId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            grade.Add(new GradeHorario
            {
                GradeHorarioId = (int)reader["GradeHorarioId"],
                TurmaId = (int)reader["TurmaId"],
                MateriaId = (int)reader["MateriaId"],
                ProfessorId = (int)reader["ProfessorId"],
                DiaSemana = (int)reader["DiaSemana"],
                HoraInicio = (TimeSpan)reader["HoraInicio"],
                HoraFim = (TimeSpan)reader["HoraFim"],
                TurmaNome = (string)reader["TurmaNome"],
                MateriaNome = (string)reader["MateriaNome"],
                ProfessorNome = (string)reader["ProfessorNome"]
            });
        }

        return grade;
    }
}
