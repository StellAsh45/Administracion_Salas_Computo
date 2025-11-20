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
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        // PANEL PRINCIPAL
        [HttpGet]
        public async Task<IActionResult> Principal()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }
    }
}