using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.ModelosSolicitud;
using System.Security.Claims;

namespace MvcSample.Controllers
{
    [Authorize]
    public class SolicitudController : Controller
    {
        private readonly ISolicitudService _solicitudService;
        private readonly IComputadorService _computadorService;

        public SolicitudController(ISolicitudService solicitudService, IComputadorService computadorService)
        {
            _solicitudService = solicitudService;
            _computadorService = computadorService;
        }

        [HttpGet]
        [Authorize(Roles = "Coordinador,Administrador")]
        public async Task<IActionResult> Index(string estado = "Pendiente")
        {
            var list = await _solicitudService.GetByEstado(estado);
            ViewBag.Estado = estado;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Forbid();

            // Filtrar solicitudes del usuario: no existe método directo en repo/service; usar GetSolicitudes y filtrar
            var all = await _solicitudService.GetSolicitudes();
            var mine = all.Where(s => s.UsuarioId == userId).ToList();
            return View(mine);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var comps = await _computadorService.GetComputadores();
            ViewBag.Computadores = comps;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AñadirModeloSolicitud model)
        {
            if (!ModelState.IsValid) return View(model);
            await _solicitudService.AddSolicitud(model);
            return RedirectToAction(nameof(MyRequests));
        }

        [HttpPost]
        [Authorize(Roles = "Coordinador,Administrador")]
        public async Task<IActionResult> Accept(Guid id)
        {
            await _solicitudService.AcceptSolicitud(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Coordinador,Administrador")]
        public async Task<IActionResult> Deny(Guid id)
        {
            await _solicitudService.DenySolicitud(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var sol = await _solicitudService.GetSolicitud(id);
            if (sol == null) return NotFound();
            return View(sol);
        }
    }
}

