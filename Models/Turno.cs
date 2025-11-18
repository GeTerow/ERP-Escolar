namespace TaskWeb.Models;

public class Turno
{
    public int TurnoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
}
