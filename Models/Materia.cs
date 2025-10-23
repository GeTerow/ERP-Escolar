namespace TaskWeb.Models;

public class Materia
{
    public int MateriaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int CargaHorariaSemanal { get; set; }
    public int TurmaId { get; set; }
    public int ProfessorId { get; set; }

    public string TurmaNome { get; set; } = string.Empty;
    public string ProfessorNome { get; set; } = string.Empty;
}
