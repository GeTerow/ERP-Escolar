namespace TaskWeb.Services;

using TaskWeb.Models;
using TaskWeb.Repositories;

public class GradeValidationService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMateriaRepository _materiaRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly ISlotAulaRepository _slotRepository;
    private readonly IDisponibilidadeProfessorRepository _disponibilidadeRepository;
    private readonly Dictionary<int, List<DisponibilidadeProfessor>> _disponibilidadeCache = new();

    public GradeValidationService(
        IGradeRepository gradeRepository,
        ITurmaRepository turmaRepository,
        IMateriaRepository materiaRepository,
        IProfessorRepository professorRepository,
        ISlotAulaRepository slotRepository,
        IDisponibilidadeProfessorRepository disponibilidadeRepository)
    {
        _gradeRepository = gradeRepository;
        _turmaRepository = turmaRepository;
        _materiaRepository = materiaRepository;
        _professorRepository = professorRepository;
        _slotRepository = slotRepository;
        _disponibilidadeRepository = disponibilidadeRepository;
    }

    public ValidationResult Validate(GradeHorario horario, int? gradeIdToIgnore = null, IEnumerable<GradeHorario>? turmaSnapshot = null)
    {
        ValidationResult result = new();
        var snapshotList = turmaSnapshot?.ToList();

        var turma = _turmaRepository.Read(horario.TurmaId);
        if (turma == null)
        {
            result.AddError("Turma nao encontrada.");
            return result;
        }

        var materia = _materiaRepository.Read(horario.MateriaId);
        if (materia == null)
        {
            result.AddError("Materia nao encontrada.");
        }
        else if (materia.TurmaId != turma.TurmaId)
        {
            result.AddError("Materia nao pertence a turma selecionada.");
        }

        var professor = _professorRepository.Read(horario.ProfessorId);
        if (professor == null)
        {
            result.AddError("Professor nao encontrado.");
        }

        if (materia != null && materia.ProfessorId != horario.ProfessorId)
        {
            result.AddError("Professor informado diverge do professor titular da materia.");
        }

        var slot = _slotRepository.Read(horario.SlotAulaId);
        if (slot == null)
        {
            result.AddError("Slot de aula nao encontrado.");
        }
        else
        {
            if (slot.EhIntervalo)
            {
                result.AddError("Nao e possivel agendar aulas em intervalos.");
            }
            if (slot.TurnoId != turma.TurnoId)
            {
                result.AddError("Slot pertence a um turno diferente da turma.");
            }
        }

        if (professor != null && slot != null)
        {
            var disponibilidade = GetDisponibilidades(horario.ProfessorId);
            if (disponibilidade.Count > 0)
            {
                bool disponivel = disponibilidade.Any(d =>
                    d.DiaSemana == horario.DiaSemana && d.SlotAulaId == horario.SlotAulaId);
                if (!disponivel)
                {
                    result.AddError("Professor nao possui disponibilidade para o dia/horario informados.");
                }
            }
        }

        var turmaGrade = snapshotList ?? _gradeRepository.ReadByTurma(horario.TurmaId);
        List<GradeHorario> professorGrade;
        if (snapshotList == null)
        {
            professorGrade = _gradeRepository.ReadByProfessor(horario.ProfessorId);
        }
        else
        {
            professorGrade = _gradeRepository
                .ReadByProfessor(horario.ProfessorId)
                .Where(g => g.TurmaId != horario.TurmaId)
                .ToList();
            professorGrade.AddRange(snapshotList.Where(g => g.ProfessorId == horario.ProfessorId));
        }

        if (turmaGrade.Any(g => g.DiaSemana == horario.DiaSemana && g.SlotAulaId == horario.SlotAulaId && g.GradeHorarioId != gradeIdToIgnore))
        {
            result.AddError("A turma ja possui aula neste dia e horario.");
        }

        if (professorGrade.Any(g => g.DiaSemana == horario.DiaSemana && g.SlotAulaId == horario.SlotAulaId && g.GradeHorarioId != gradeIdToIgnore))
        {
            result.AddError("O professor ja esta alocado neste horario.");
        }

        if (materia != null && materia.CargaHorariaSemanal > 0)
        {
            int aulasAgendadas = turmaGrade.Count(g => g.MateriaId == materia.MateriaId && g.GradeHorarioId != gradeIdToIgnore);
            if (aulasAgendadas >= materia.CargaHorariaSemanal)
            {
                result.AddError("Carga horaria semanal da materia ja foi atendida.");
            }
        }

        return result;
    }

    private List<DisponibilidadeProfessor> GetDisponibilidades(int professorId)
    {
        if (_disponibilidadeCache.TryGetValue(professorId, out var cache))
        {
            return cache;
        }

        var lista = _disponibilidadeRepository.ReadByProfessor(professorId);
        _disponibilidadeCache[professorId] = lista;
        return lista;
    }
}

