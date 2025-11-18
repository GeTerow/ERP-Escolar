namespace TaskWeb.Models;

public class SlotAula
{
    public int SlotAulaId { get; set; }
    public int TurnoId { get; set; }
    public int Sequencia { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    public bool EhIntervalo { get; set; }

    public string TurnoNome { get; set; } = string.Empty;
}
