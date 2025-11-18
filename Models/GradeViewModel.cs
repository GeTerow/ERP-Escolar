namespace TaskWeb.Models;

public class GradeViewModel
{
    public List<Turma> Turmas { get; set; } = new();
    public int TurmaSelecionadaId { get; set; }
    public List<GradeLinhaViewModel> Linhas { get; set; } = new();
}

public class GradeLinhaViewModel
{
    public int SlotAulaId { get; set; }
    public int Sequencia { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    public bool EhIntervalo { get; set; }
    public Dictionary<int, GradeHorario?> AulasPorDia { get; set; } = new();
}
