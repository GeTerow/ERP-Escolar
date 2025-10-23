using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class AlunosController : BaseController
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ITurmaRepository _turmaRepository;

    public AlunosController(IAlunoRepository alunoRepository, ITurmaRepository turmaRepository)
    {
        _alunoRepository = alunoRepository;
        _turmaRepository = turmaRepository;
    }

    public IActionResult Index()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        return View(_alunoRepository.ReadAll());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        PopularTurmas();
        return View(new Aluno());
    }

    [HttpPost]
    public IActionResult Create(Aluno aluno)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(aluno.Nome))
        {
            ViewBag.Error = "Informe Nome.";
            PopularTurmas();
            return View(aluno);
        }
        if (string.IsNullOrWhiteSpace(aluno.Matricula))
        {
            ViewBag.Error = "Informe matrícula.";
            PopularTurmas();
            return View(aluno);
        }
        if (aluno.TurmaId == 0)
        {
            ViewBag.Error = "Informe a turma.";
            PopularTurmas();
            return View(aluno);
        }

        _alunoRepository.Create(aluno);
        TempData["Success"] = "Aluno cadastrado com sucesso.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var aluno = _alunoRepository.Read(id);
        if (aluno == null)
        {
            return NotFound();
        }

        PopularTurmas();
        return View(aluno);
    }

    [HttpPost]
    public IActionResult Edit(Aluno aluno)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        if (string.IsNullOrWhiteSpace(aluno.Nome))
        {
            ViewBag.Error = "Informe Nome.";
            PopularTurmas();
            return View(aluno);
        }
        if (string.IsNullOrWhiteSpace(aluno.Matricula))
        {
            ViewBag.Error = "Informe matrícula.";
            PopularTurmas();
            return View(aluno);
        }
        if (aluno.TurmaId == 0)
        {
            ViewBag.Error = "Informe a turma.";
            PopularTurmas();
            return View(aluno);
        }

            _alunoRepository.Update(aluno);
            TempData["Success"] = "Aluno atualizado com sucesso.";
            return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!UsuarioLogado())
        {
            return RedirectToAction("Login", "Usuario");
        }

        var aluno = _alunoRepository.Read(id);
        if (aluno == null)
        {
            return NotFound();
        }

        return View(aluno);
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
            _alunoRepository.Delete(id);
            TempData["Success"] = "Aluno removido com sucesso.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Nao foi possivel remover o aluno. Verifique vinculos existentes.";
        }

        return RedirectToAction("Index");
    }

    private void PopularTurmas()
    {
        ViewBag.Turmas = _turmaRepository.ReadAll();
    }
}
