using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;
using TaskWeb.Services;

namespace TaskWeb.Controllers;

public class GradeController : BaseController
{
    private readonly IGradeRepository _gradeRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly ISlotAulaRepository _slotRepository;
    private readonly GradeGenerationService _gradeGenerationService;

    public GradeController(
        IGradeRepository gradeRepository,
        ITurmaRepository turmaRepository,
        ISlotAulaRepository slotRepository,
        GradeGenerationService gradeGenerationService)
    {
        _gradeRepository = gradeRepository;
        _turmaRepository = turmaRepository;
        _slotRepository = slotRepository;
        _gradeGenerationService = gradeGenerationService;
    }

    [HttpGet]
    public IActionResult Index(int? turmaId)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var turmas = _turmaRepository.ReadAll();
        if (turmas.Count == 0)
        {
            return View(new GradeViewModel());
        }

        int turmaSelecionada = turmaId ?? turmas[0].TurmaId;
        var turmaAtual = turmas.FirstOrDefault(t => t.TurmaId == turmaSelecionada) ?? turmas[0];
        var grade = _gradeRepository.ReadByTurma(turmaAtual.TurmaId);
        var slots = _slotRepository.ReadByTurno(turmaAtual.TurnoId).OrderBy(s => s.Sequencia).ToList();
        if (slots.Count == 0)
        {
            slots = new List<SlotAula>();
        }

        List<GradeLinhaViewModel> linhas = new();
        foreach (var slot in slots)
        {
            Dictionary<int, GradeHorario?> aulasPorDia = new();
            for (int dia = 1; dia <= 5; dia++)
            {
                var entrada = grade.FirstOrDefault(g => g.DiaSemana == dia && g.SlotAulaId == slot.SlotAulaId);
                aulasPorDia[dia] = entrada;
            }

            linhas.Add(new GradeLinhaViewModel
            {
                SlotAulaId = slot.SlotAulaId,
                Sequencia = slot.Sequencia,
                HoraInicio = slot.HoraInicio,
                HoraFim = slot.HoraFim,
                EhIntervalo = slot.EhIntervalo,
                AulasPorDia = aulasPorDia
            });
        }

        var model = new GradeViewModel
        {
            Turmas = turmas,
            TurmaSelecionadaId = turmaAtual.TurmaId,
            Linhas = linhas
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Gerar(int turmaId)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var resultado = _gradeGenerationService.GerarParaTurma(turmaId);
        if (!resultado.Success)
        {
            TempData["Error"] = string.Join(" ", resultado.Errors);
        }
        else
        {
            TempData["Success"] = $"Grade gerada com {resultado.HorariosGerados.Count} aulas.";
        }

        return RedirectToAction("Index", new { turmaId });
    }
}
