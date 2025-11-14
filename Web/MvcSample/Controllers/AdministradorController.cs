using Domain;
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
        public async Task<IActionResult> VerUsuarios()
        {
            var usuarios = await _usuarioService.GetUsuarios();
            usuarios ??= new List<ModeloUsuario>();
            return View(usuarios);
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

            try
            {
                if (string.IsNullOrWhiteSpace(model.Rol)) model.Rol = "Usuario";
                await _usuarioService.AddUsuario(model);
                TempData["Success"] = "Registro exitoso. El usuario fue creado correctamente.";
                return RedirectToAction("VerUsuarios");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("RegistroUsuarios", model);
            }
        }

        // Editar - GET
        [HttpGet]
        public async Task<IActionResult> EditarUsuario(Guid id)
        {
            var usuario = await _usuarioService.GetUsuario(id);
            if (usuario == null) return NotFound();
            return View("EditarUsuario", usuario); 
        }

        // Editar - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(ModeloUsuario model)
        {
            if (!ModelState.IsValid) return View("EditarUsuario", model); 

            try
            {
                await _usuarioService.UpdateUsuario(model);
                TempData["Success"] = "Usuario actualizado correctamente.";
                return RedirectToAction("VerUsuarios");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("EditarUsuario", model); 
            }
        }

        // Eliminar - POST
        [HttpPost, ActionName("BorrarUsuario")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrarUsuarioConfirmed(Guid id)
        {
            await _usuarioService.DeleteUsuario(id);
            TempData["Success"] = "Usuario eliminado correctamente.";
            return RedirectToAction("VerUsuarios");
        }
    }
}
