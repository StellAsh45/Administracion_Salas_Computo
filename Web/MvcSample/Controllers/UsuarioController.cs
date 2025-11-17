using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.ModelosUsuario;
using Services.Models.ModelosComputador;

namespace MvcSample.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class UsuarioController : Controller
    {
        // PANEL PRINCIPAL
        public IActionResult Principal()
        {
            return View();
        }
    }
}