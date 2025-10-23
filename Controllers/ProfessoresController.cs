using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class ProfessoresController : BaseController
{
    private readonly IProfessorRepository _professorRepository;

    public ProfessoresController(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
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

}
