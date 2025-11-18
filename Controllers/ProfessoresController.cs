using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class ProfessoresController : BaseController
{
    private readonly IProfessorRepository _professorRepository;
    private readonly ISlotAulaRepository _slotRepository;
    private readonly IDisponibilidadeProfessorRepository _disponibilidadeRepository;
    private static readonly DiaSemanaOption[] _diasSemana = new[]
    {
        new DiaSemanaOption { Valor = 1, Nome = "Segunda" },
        new DiaSemanaOption { Valor = 2, Nome = "Terca" },
        new DiaSemanaOption { Valor = 3, Nome = "Quarta" },
        new DiaSemanaOption { Valor = 4, Nome = "Quinta" },
        new DiaSemanaOption { Valor = 5, Nome = "Sexta" }
    };

    public ProfessoresController(
        IProfessorRepository professorRepository,
        ISlotAulaRepository slotRepository,
        IDisponibilidadeProfessorRepository disponibilidadeRepository)
    {
        _professorRepository = professorRepository;
        _slotRepository = slotRepository;
        _disponibilidadeRepository = disponibilidadeRepository;
    }

    public IActionResult Index()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        return View(_professorRepository.ReadAll());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        return View(new Professor());
    }

    [HttpPost]
    public IActionResult Create(Professor professor)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(professor.Nome))
        {
            ViewBag.Error = "Informe o nome do professor.";
            return View(professor);
        }

        if (string.IsNullOrWhiteSpace(professor.Email))
        {
            ViewBag.Error = "Informe o email do professor.";
            return View(professor);
        }
        if (string.IsNullOrWhiteSpace(professor.Telefone))
        {
            ViewBag.Error = "Informe o telefone e do professor.";
            return View(professor);
        }

        _professorRepository.Create(professor);
        TempData["Success"] = "Professor cadastrado com sucesso.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var professor = _professorRepository.Read(id);
        if (professor == null)
        {
            return NotFound();
        }

        return View(professor);
    }

    [HttpPost]
    public IActionResult Edit(Professor professor)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(professor.Nome))
        {
            ViewBag.Error = "Informe o nome do professor.";
            return View(professor);
        }

        if (string.IsNullOrWhiteSpace(professor.Email))
        {
            ViewBag.Error = "Informe o email do professor.";
            return View(professor);
        }
        if (string.IsNullOrWhiteSpace(professor.Telefone))
        {
            ViewBag.Error = "Informe o telefone e do professor.";
            return View(professor);
        }

        try
        {
            _professorRepository.Update(professor);
            TempData["Success"] = "Professor atualizado com sucesso.";
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            ViewBag.Error = "Nao foi possivel atualizar o professor. Tente novamente.";
            return View(professor);
        }
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var professor = _professorRepository.Read(id);
        if (professor == null)
        {
            return NotFound();
        }

        return View(professor);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        try
        {
            _professorRepository.Delete(id);
            TempData["Success"] = "Professor removido com sucesso.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Nao foi possivel remover o professor. Verifique vinculos existentes.";
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Disponibilidade(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var professor = _professorRepository.Read(id);
        if (professor == null)
        {
            return NotFound();
        }

        var slots = _slotRepository
            .ReadAll()
            .Where(s => !s.EhIntervalo)
            .OrderBy(s => s.TurnoNome)
            .ThenBy(s => s.Sequencia)
            .ToList();

        var selecionados = _disponibilidadeRepository
            .ReadByProfessor(id)
            .GroupBy(d => d.DiaSemana)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.SlotAulaId).ToHashSet());

        ProfessorDisponibilidadeViewModel model = new()
        {
            ProfessorId = professor.ProfessorId,
            ProfessorNome = professor.Nome,
            Slots = slots,
            Dias = _diasSemana.Select(d => new DiaSemanaOption { Valor = d.Valor, Nome = d.Nome }).ToList(),
            Selecionados = selecionados
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Disponibilidade(ProfessorDisponibilidadeInput input)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var professor = _professorRepository.Read(input.ProfessorId);
        if (professor == null)
        {
            return NotFound();
        }

        var diasValidos = _diasSemana.Select(d => d.Valor).ToHashSet();
        var slotsValidos = _slotRepository
            .ReadAll()
            .Where(s => !s.EhIntervalo)
            .Select(s => s.SlotAulaId)
            .ToHashSet();

        _disponibilidadeRepository.DeleteByProfessor(input.ProfessorId);

        if (input.Selecionados != null)
        {
            foreach (var token in input.Selecionados.Distinct())
            {
                var parts = token.Split(':');
                if (parts.Length != 2)
                {
                    continue;
                }

                if (!int.TryParse(parts[0], out int dia) || !int.TryParse(parts[1], out int slotId))
                {
                    continue;
                }

                if (!diasValidos.Contains(dia) || !slotsValidos.Contains(slotId))
                {
                    continue;
                }

                _disponibilidadeRepository.Create(new DisponibilidadeProfessor
                {
                    ProfessorId = input.ProfessorId,
                    DiaSemana = dia,
                    SlotAulaId = slotId
                });
            }
        }

        TempData["Success"] = "Disponibilidade atualizada.";
        return RedirectToAction("Disponibilidade", new { id = input.ProfessorId });
    }
}
