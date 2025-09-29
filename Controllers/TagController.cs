using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;

namespace TaskWeb.Controllers;

public class TagController : Controller
{
    static List<Tag> lista = new List<Tag>();

    public ActionResult Index()
    {
        return View(lista);
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

        lista.Add(tag);
        return RedirectToAction("Index");
    }
}