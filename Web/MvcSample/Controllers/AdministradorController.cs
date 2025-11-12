using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.ModelosUsuario;

namespace MvcSample.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministradorController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AdministradorController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Principal()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }

        [HttpGet]
        public IActionResult RegistroUsuarios()
        {
            return View(new AñadirModeloUsuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroUsuarios(AñadirModeloUsuario model)
        {
            if (!ModelState.IsValid) return View("RegistroUsuarios", model);

            if (string.IsNullOrWhiteSpace(model.Rol)) model.Rol = "Usuario";
            await _usuarioService.AddUsuario(model);
            TempData["Success"] = "Registro exitoso. El usuario fue creado correctamente.";

            return RedirectToAction("Principal");
        }
    }
}
