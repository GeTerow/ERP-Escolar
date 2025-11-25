namespace TaskWeb.Services;

using TaskWeb.Models;
using TaskWeb.Repositories;
using System.Linq;

public class GradeGenerationService
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMateriaRepository _materiaRepository;
    private readonly ISlotAulaRepository _slotRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly GradeValidationService _validationService;

    public GradeGenerationService(
        ITurmaRepository turmaRepository,
        IMateriaRepository materiaRepository,
        ISlotAulaRepository slotRepository,
        IGradeRepository gradeRepository,
        GradeValidationService validationService)
    {
        _turmaRepository = turmaRepository;
        _materiaRepository = materiaRepository;
        _slotRepository = slotRepository;
        _gradeRepository = gradeRepository;
        _validationService = validationService;
    }

    public GradeGenerationResult GerarParaTurma(int turmaId)
    {
        GradeGenerationResult result = new();

        var turma = _turmaRepository.Read(turmaId);
        if (turma == null)
        {
            result.AddError("Turma não encontrada.");
            return result;
        }

        var materias = _materiaRepository.ReadByTurma(turmaId);
        if (materias.Count == 0)
        {
            result.AddError("Cadastre matérias para a turma antes de gerar a grade.");
            return result;
        }

        var slots = _slotRepository.ReadByTurno(turma.TurnoId)
            .Where(s => !s.EhIntervalo)
            .OrderBy(s => s.Sequencia)
            .ToList();
        if (slots.Count == 0)
        {
            result.AddError("Não existem slots cadastrados para o turno da turma.");
            return result;
        }

        var alocacoes = materias
            .SelectMany(m => Enumerable.Repeat(m, Math.Max(1, m.CargaHorariaSemanal)))
            .OrderByDescending(m => m.CargaHorariaSemanal)
            .ThenBy(m => m.Nome)
            .ToList();

        List<GradeHorario> snapshot = new();
        HashSet<(int Dia, int SlotId)> turmaOcupacao = new();
        Dictionary<int, HashSet<(int Dia, int SlotId)>> professorOcupacao = new();
        int alocacaoIndex = 0;
        var dias = Enumerable.Range(1, 5).ToList();

        foreach (var materia in alocacoes)
        {
            bool alocada = false;
            HashSet<string> motivosDeFalha = new(); 

            var orderedDias = RotateDias(dias, alocacaoIndex);
            
            foreach (var dia in orderedDias)
            {
                foreach (var slot in slots)
                {
                    if (turmaOcupacao.Contains((dia, slot.SlotAulaId)))
                    {
                        continue;
                    }

                    if (professorOcupacao.TryGetValue(materia.ProfessorId, out var profSlots) && profSlots.Contains((dia, slot.SlotAulaId)))
                    {
                        motivosDeFalha.Add($"Professor {materia.ProfessorNome} já tem aula neste horário");
                        continue;
                    }

                    var novoHorario = new GradeHorario
                    {
                        TurmaId = turma.TurmaId,
                        MateriaId = materia.MateriaId,
                        ProfessorId = materia.ProfessorId,
                        SlotAulaId = slot.SlotAulaId,
                        DiaSemana = dia,
                        SlotSequencia = slot.Sequencia,
                        SlotEhIntervalo = slot.EhIntervalo,
                        HoraInicio = slot.HoraInicio,
                        HoraFim = slot.HoraFim,
                        TurmaNome = turma.Nome,
                        MateriaNome = materia.Nome,
                        ProfessorNome = materia.ProfessorNome
                    };

                    var validation = _validationService.Validate(novoHorario, null, snapshot);
                    
                    if (validation.Success)
                    {
                        snapshot.Add(novoHorario);
                        turmaOcupacao.Add((dia, slot.SlotAulaId));
                        
                        if (!professorOcupacao.TryGetValue(materia.ProfessorId, out var slotsProfessor))
                        {
                            slotsProfessor = new HashSet<(int, int)>();
                            professorOcupacao[materia.ProfessorId] = slotsProfessor;
                        }
                        slotsProfessor.Add((dia, slot.SlotAulaId));
                        
                        alocada = true;
                        break;
                    }
                    else
                    {
                        foreach(var erro in validation.Errors) motivosDeFalha.Add(erro);
                    }
                }

                if (alocada) break;
            }

            if (!alocada)
            {
                string detalhes = motivosDeFalha.Count > 0 
                    ? string.Join("; ", motivosDeFalha) 
                    : "Sem horários vagos ou conflito geral.";
                result.AddError($"Falha ao alocar {materia.Nome}: {detalhes}");
            }

            alocacaoIndex++;
        }
        if (snapshot.Count > 0)
        {
            _gradeRepository.DeleteByTurma(turmaId);
            foreach (var horario in snapshot)
            {
                _gradeRepository.Create(horario);
            }
            result.HorariosGerados.AddRange(_gradeRepository.ReadByTurma(turmaId));
        }
        else if (result.Errors.Count == 0)
        {
            result.AddError("Não foi possível gerar nenhuma aula com os parâmetros atuais.");
        }

        return result;
    }

    private static IEnumerable<int> RotateDias(List<int> dias, int offset)
    {
        int count = dias.Count;
        for (int i = 0; i < count; i++)
        {
            yield return dias[(i + offset) % count];
        }
    }
}