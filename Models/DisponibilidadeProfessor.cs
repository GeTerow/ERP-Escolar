namespace TaskWeb.Models;

public class DisponibilidadeProfessor
{
    public int DisponibilidadeProfessorId { get; set; }
    public int ProfessorId { get; set; }
    public int DiaSemana { get; set; }
    public int SlotAulaId { get; set; }
    public int SlotSequencia { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    public string ProfessorNome { get; set; } = string.Empty;
}
