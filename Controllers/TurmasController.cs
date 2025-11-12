using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class TurmasController : BaseController
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly ITurnoRepository _turnoRepository;

    public TurmasController(ITurmaRepository turmaRepository, ITurnoRepository turnoRepository)
    {
        _turmaRepository = turmaRepository;
        _turnoRepository = turnoRepository;
    }

    public IActionResult Index()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        return View(_turmaRepository.ReadAll());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        CarregarTurnos();
        return View(new Turma());
    }

    [HttpPost]
    public IActionResult Create(Turma turma)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(turma.Nome))
        {
            CarregarTurnos();
            ViewBag.Error = "Informe o nome da turma.";
            return View(turma);
        }
        if (string.IsNullOrWhiteSpace(turma.AnoLetivo))
        {
            CarregarTurnos();
            ViewBag.Error = "Informe o ano letivo da turma.";
            return View(turma);
        }
        if (turma.TurnoId <= 0)
        {
            CarregarTurnos();
            ViewBag.Error = "Selecione um turno.";
            return View(turma);
        }

        _turmaRepository.Create(turma);
        TempData["Success"] = "Turma cadastrada com sucesso.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var turma = _turmaRepository.Read(id);
        if (turma == null)
        {
            return NotFound();
        }

        CarregarTurnos();
        return View(turma);
    }

    [HttpPost]
    public IActionResult Edit(Turma turma)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(turma.Nome))
        {
            CarregarTurnos();
            ViewBag.Error = "Informe o nome da turma.";
            return View(turma);
        }
        if (string.IsNullOrWhiteSpace(turma.AnoLetivo))
        {
            CarregarTurnos();
            ViewBag.Error = "Informe o ano letivo da turma.";
            return View(turma);
        }
        if (turma.TurnoId <= 0)
        {
            CarregarTurnos();
            ViewBag.Error = "Selecione um turno.";
            return View(turma);
        }

        try
        {
            _turmaRepository.Update(turma);
            TempData["Success"] = "Turma atualizada com sucesso.";
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            CarregarTurnos();
            ViewBag.Error = "Nao foi possivel atualizar a turma. Tente novamente.";
            return View(turma);
        }
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var turma = _turmaRepository.Read(id);
        if (turma == null)
        {
            return NotFound();
        }

        return View(turma);
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
            _turmaRepository.Delete(id);
            TempData["Success"] = "Turma removida com sucesso.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Nao foi possivel remover a turma.";
        }

        return RedirectToAction("Index");
    }

    private void CarregarTurnos()
    {
        ViewBag.Turnos = _turnoRepository.ReadAll();
    }
}



