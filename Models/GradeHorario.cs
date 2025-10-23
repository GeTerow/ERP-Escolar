namespace TaskWeb.Models;

public class GradeHorario
{
    public int GradeHorarioId { get; set; }
    public int TurmaId { get; set; }
    public int MateriaId { get; set; }
    public int ProfessorId { get; set; }
    public int DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }

    public string TurmaNome { get; set; } = string.Empty;
    public string MateriaNome { get; set; } = string.Empty;
    public string ProfessorNome { get; set; } = string.Empty;
}
