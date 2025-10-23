namespace TaskWeb.Models;

public class GradeViewModel
{
    public List<Turma> Turmas { get; set; } = new();
    public int TurmaSelecionadaId { get; set; }
    public List<GradeLinhaViewModel> Linhas { get; set; } = new();
}

public class GradeLinhaViewModel
{
    public TimeSpan Hora { get; set; }
    public Dictionary<int, GradeHorario?> AulasPorDia { get; set; } = new();
}
