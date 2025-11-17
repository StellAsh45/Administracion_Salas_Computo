using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MvcSample.Controllers
{
    [Authorize(Roles = "Coordinador de Sala")]
    public class CoordinadorSalaController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public CoordinadorSalaController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Principal()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }
    }
}
