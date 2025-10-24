using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class GradeController : BaseController
{
    private IGradeRepository _gradeRepository;
    private ITurmaRepository _turmaRepository;

    public GradeController(
        IGradeRepository gradeRepository,
        ITurmaRepository turmaRepository)
    {
        _gradeRepository = gradeRepository;
        _turmaRepository = turmaRepository;
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
        var grade = _gradeRepository.ReadByTurma(turmaSelecionada);

        var horarios = grade.Select(g => g.HoraInicio).Distinct().OrderBy(h => h).ToList();
        if (horarios.Count == 0)
        {
            horarios = new List<TimeSpan>
            {
                TimeSpan.FromHours(8),
                TimeSpan.FromHours(9),
                TimeSpan.FromHours(10),
                TimeSpan.FromHours(11),
                TimeSpan.FromHours(13),
                TimeSpan.FromHours(14)
            };
        }

        List<GradeLinhaViewModel> linhas = new();
        foreach (var horario in horarios)
        {
            Dictionary<int, GradeHorario?> aulasPorDia = new();
            for (int dia = 1; dia <= 5; dia++)
            {
                var entrada = grade.FirstOrDefault(g => g.DiaSemana == dia && g.HoraInicio == horario);
                aulasPorDia[dia] = entrada;
            }

            linhas.Add(new GradeLinhaViewModel
            {
                Hora = horario,
                AulasPorDia = aulasPorDia
            });
        }

        var model = new GradeViewModel
        {
            Turmas = turmas,
            TurmaSelecionadaId = turmaSelecionada,
            Linhas = linhas
        };

        return View(model);
    }

}
