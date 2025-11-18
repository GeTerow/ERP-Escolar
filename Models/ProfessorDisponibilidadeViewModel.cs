using System.Collections.Generic;

namespace TaskWeb.Models;

public class ProfessorDisponibilidadeViewModel
{
    public int ProfessorId { get; set; }
    public string ProfessorNome { get; set; } = string.Empty;
    public List<SlotAula> Slots { get; set; } = new();
    public List<DiaSemanaOption> Dias { get; set; } = new();
    public Dictionary<int, HashSet<int>> Selecionados { get; set; } = new();
}

public class DiaSemanaOption
{
    public int Valor { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ProfessorDisponibilidadeInput
{
    public int ProfessorId { get; set; }
    public List<string> Selecionados { get; set; } = new();
}

