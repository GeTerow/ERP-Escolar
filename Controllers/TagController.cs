using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class TagController : Controller
{

    private ITagRepository repository;

    public TagController(ITagRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index()
    {
        return View(repository.Read());
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(Tag tag)
    {
        //string title = form["Title"];
        //Tag tag = new Tag();
        //tag.Title = title;

        repository.Create(tag);
        return RedirectToAction("Index");
    }
}