using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWeb.Models;
using TaskWeb.Repositories;

namespace TaskWeb.Controllers;

public class UsuarioController : BaseController
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet]
    public ActionResult Login()
    {
        if (UsuarioLogado()) return RedirectToAction("Index", "Home");
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

        var usuario = _usuarioRepository.Login(model);

        if (usuario == null)
        {
            ViewBag.Error = "Usuário ou senha inválidos";
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

    public IActionResult Index()
    {
        if (!UsuarioLogado()) return RedirectToAction("Login");

        return View(_usuarioRepository.ReadAll());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!UsuarioLogado()) return RedirectToAction("Login");
        return View(new Usuario());
    }

    [HttpPost]
    public IActionResult Create(Usuario usuario)
    {
        if (!UsuarioLogado()) return RedirectToAction("Login");

        if (!ValidarUsuario(usuario)) return View(usuario);

        try
        {
            _usuarioRepository.Create(usuario);
            TempData["Success"] = "Usuário cadastrado com sucesso.";
            return RedirectToAction("Index");
        }
        catch
        {
            ViewBag.Error = "Erro ao cadastrar usuário. Tente outro email.";
            return View(usuario);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!UsuarioLogado()) return RedirectToAction("Login");

        var usuario = _usuarioRepository.Read(id);
        if (usuario == null) return NotFound();

        return View(usuario);
    }

    [HttpPost]
    public IActionResult Edit(Usuario usuario)
    {
        if (!UsuarioLogado()) return RedirectToAction("Login");

        if (!ValidarUsuario(usuario)) return View(usuario);

        try
        {
            _usuarioRepository.Update(usuario);
            TempData["Success"] = "Usuário atualizado com sucesso.";
            return RedirectToAction("Index");
        }
        catch
        {
            ViewBag.Error = "Erro ao atualizar usuário.";
            return View(usuario);
        }
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!UsuarioLogado()) return RedirectToAction("Login");

        var usuario = _usuarioRepository.Read(id);
        if (usuario == null) return NotFound();

        return View(usuario);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!UsuarioLogado()) return RedirectToAction("Login");

        var usuarioLogadoId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioLogadoId == id)
        {
            TempData["Error"] = "Você não pode excluir seu próprio usuário enquanto está logado.";
            return RedirectToAction("Index");
        }

        _usuarioRepository.Delete(id);
        TempData["Success"] = "Usuário removido com sucesso.";
        return RedirectToAction("Index");
    }

    private bool ValidarUsuario(Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nome))
        {
            ViewBag.Error = "O nome é obrigatório.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(usuario.Email))
        {
            ViewBag.Error = "O e-mail é obrigatório.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(usuario.Senha))
        {
            ViewBag.Error = "A senha é obrigatória.";
            return false;
        }
        return true;
    }
}