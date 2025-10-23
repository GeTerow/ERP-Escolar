namespace TaskWeb.Models;

public class Aluno
{
    public int AlunoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public int TurmaId { get; set; }

    // Apenas para exibicao
    public string TurmaNome { get; set; } = string.Empty;
}
