using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;

namespace TaskWeb.Controllers;

public class TagController : Controller
{
    public ActionResult Index()
    {
        List<Tag> lista = new List<Tag>();
        lista.Add(new Tag { TagId = 1, Title = "Estudo" });
        lista.Add(new Tag { TagId = 2, Title = "Trabalho" });
        lista.Add(new Tag { TagId = 3, Title = "Jogos" });
        lista.Add(new Tag { TagId = 3, Title = "Família" });

        return View(lista);
    }
}