using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.ModelosSala;

namespace MvcSample.Controllers
{
    [Authorize]
    public class SalaController : Controller
    {
        private readonly ISalaService _salaService;

        public SalaController(ISalaService salaService)
        {
            _salaService = salaService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var salas = await _salaService.GetSalas();
            return View(salas);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var sala = await _salaService.GetSala(id);
            if (sala == null) return NotFound();
            ViewBag.Computadores = await _salaService.GetComputadoresBySala(id);
            return View(sala);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(AñadirModeloSala model)
        {
            if (!ModelState.IsValid) return View(model);
            await _salaService.AddSala(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var sala = await _salaService.GetSala(id);
            if (sala == null) return NotFound();
            return View(sala);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(ModeloSala model)
        {
            if (!ModelState.IsValid) return View(model);
            await _salaService.UpdateSala(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var sala = await _salaService.GetSala(id);
            if (sala == null) return NotFound();
            return View(sala);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _salaService.DeleteSala(id);
            return RedirectToAction(nameof(Index));
        }
    }
}