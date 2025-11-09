using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.ModelosUsuario;

namespace MvcSample.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _usuarioService.GetUsuarios();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AñadirModeloUsuario model)
        {
            if (!ModelState.IsValid) return View(model);
            await _usuarioService.AddUsuario(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _usuarioService.GetUsuario(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ModeloUsuario model)
        {
            if (!ModelState.IsValid) return View(model);
            await _usuarioService.UpdateUsuario(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _usuarioService.GetUsuario(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _usuarioService.DeleteUsuario(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(Guid usuarioId, string role)
        {
            await _usuarioService.AssignRole(usuarioId, role);
            return RedirectToAction(nameof(Edit), new { id = usuarioId });
        }
    }
}

