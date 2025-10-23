using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class UsuarioController : Controller
{
    private readonly IUsuarioRepository repository;

    public UsuarioController(IUsuarioRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public ActionResult Login(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
        {
            ViewBag.Error = "Informe o email.";
            return View(model);
        }
        if (string.IsNullOrWhiteSpace(model.Senha))
        {
            ViewBag.Error = "Informe a senha.";
            return View(model);
        }

        var usuario = repository.Login(model);

        if (usuario == null)
        {
            ViewBag.Error = "Usuario ou senha invalidos";
            return View(model);
        }

        HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
        HttpContext.Session.SetString("Nome", usuario.Nome);

        return RedirectToAction("Index", "Home");
    }

    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
