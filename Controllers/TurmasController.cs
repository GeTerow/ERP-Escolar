using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class TurmasController : BaseController
{
    private readonly ITurmaRepository _turmaRepository;

    public TurmasController(ITurmaRepository turmaRepository)
    {
        _turmaRepository = turmaRepository;
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
            ViewBag.Error = "Informe o nome da turma.";
            return View(turma);
        }
        if (string.IsNullOrWhiteSpace(turma.AnoLetivo))
        {
            ViewBag.Error = "Informe o ano letivo da turma.";
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
            ViewBag.Error = "Informe o nome da turma.";
            return View(turma);
        }
        if (string.IsNullOrWhiteSpace(turma.AnoLetivo))
        {
            ViewBag.Error = "Informe o ano letivo da turma.";
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
            TempData["Error"] = "Nao foi possivel remover a turma. Verifique vinculos existentes.";
        }

        return RedirectToAction("Index");
    }

}
