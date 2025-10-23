using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool UsuarioLogado()
        {
            return HttpContext.Session.GetInt32("UsuarioId") != null;
        }
    }
}