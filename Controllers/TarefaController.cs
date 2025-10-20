using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class TarefaController : Controller
{

    private ITarefaRepository repository;
    private int usuarioId;

    public TarefaController(ITarefaRepository repository)
    {
        this.repository = repository;
        usuarioId = HttpContext.Session.GetInt32("UsuarioId");
    }

    public ActionResult Index()
    {
        return View(repository.Read(usuarioId));
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(Tarefa model)
    {
        model.UsuarioId = usuarioId;

        repository.Create(model);
        return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
        repository.Delete(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int id)
    {
        var Tarefa = repository.Read(id);
        return View(Tarefa);
    }

    [HttpPost]
    public ActionResult Update(int id, Tarefa model)
    {
        Tarefa.TarefaId = id;
        repository.Update(model);

        return RedirectToAction("Index");
    }
}