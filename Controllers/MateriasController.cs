using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class MateriasController : BaseController
{
    private IMateriaRepository _materiaRepository;
    private ITurmaRepository _turmaRepository;
    private IProfessorRepository _professorRepository;

    public MateriasController(
        IMateriaRepository materiaRepository,
        ITurmaRepository turmaRepository,
        IProfessorRepository professorRepository)
    {
        _materiaRepository = materiaRepository;
        _turmaRepository = turmaRepository;
        _professorRepository = professorRepository;
    }

    public IActionResult Index()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        return View(_materiaRepository.ReadAll());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        PopularListas();
        return View(new Materia());
    }

    [HttpPost]
    public IActionResult Create(Materia materia)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(materia.Nome))
        {
            ViewBag.Error = "Informe o nome da matéria.";
            PopularListas();
            return View(materia);
        }
        if (materia.CargaHorariaSemanal <=0)
        {
            ViewBag.Error = "Informe a carga horária da semana (minímo 1).";
            PopularListas();
            return View(materia);
        }
        if (materia.TurmaId == 0)
        {
            ViewBag.Error = "Informe a turma.";
            PopularListas();
            return View(materia);
        }
        if (materia.ProfessorId == 0)
        {
            ViewBag.Error = "Informe o professor responsável.";
            PopularListas();
            return View(materia);
        }
        _materiaRepository.Create(materia);
        TempData["Success"] = "Matéria cadastrada com sucesso.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var materia = _materiaRepository.Read(id);
        if (materia == null)
        {
            return NotFound();
        }

        PopularListas();
        return View(materia);
    }

    [HttpPost]
    public IActionResult Edit(Materia materia)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(materia.Nome))
        {
            ViewBag.Error = "Informe o nome da matéria.";
            PopularListas();
            return View(materia);
        }
        if (materia.CargaHorariaSemanal <=0)
        {
            ViewBag.Error = "Informe a carga horária da semana (minímo 1).";
            PopularListas();
            return View(materia);
        }
        if (materia.TurmaId == 0)
        {
            ViewBag.Error = "Informe a turma.";
            PopularListas();
            return View(materia);
        }
        if (materia.ProfessorId == 0)
        {
            ViewBag.Error = "Informe o professor responsável.";
            PopularListas();
            return View(materia);
        }

        _materiaRepository.Update(materia);
        TempData["Success"] = "Materia atualizada com sucesso.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var materia = _materiaRepository.Read(id);
        if (materia == null)
        {
            return NotFound();
        }

        return View(materia);
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
            _materiaRepository.Delete(id);
            TempData["Success"] = "Materia removida com sucesso.";
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            TempData["Error"] = "Nao foi possivel remover a matéria. Tente novamente.";
            return RedirectToAction("Index");
        }

    }

    private void PopularListas()
    {
        ViewBag.Turmas = _turmaRepository.ReadAll();
        ViewBag.Professores = _professorRepository.ReadAll();
    }

}
