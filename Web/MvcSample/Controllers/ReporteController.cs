using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.ModelosReporte;
using System.Security.Claims;

namespace MvcSample.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet]
        [Authorize(Roles = "Coordinador,Administrador")]
        public async Task<IActionResult> Index()
        {
            var list = await _reporteService.GetReportes();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AñadirModeloReporte model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.UsuarioId == Guid.Empty)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdStr, out var u)) model.UsuarioId = u;
            }

            await _reporteService.AddReporte(model);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var r = await _reporteService.GetReporte(id);
            if (r == null) return NotFound();
            return View(r);
        }

        [HttpPost]
        [Authorize(Roles = "Coordinador,Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _reporteService.DeleteReporte(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

