using Microsoft.AspNetCore.Mvc;

namespace TaskWeb.Controllers;

public class TagController : Controller
{
    public ActionResult Index()
    {
        return Ok("Hello, World!");
    }

    public ActionResult Translate()
    {
        return NotFound();
    }
}