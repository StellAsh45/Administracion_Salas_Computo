using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.Models.ModelosComputador;
using Services.Models.ModelosSolicitud;
using System;
using System.Linq;
using System.Security.Claims;

namespace MvcSample.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IComputadorService _computadorService;
        private readonly ISalaService _salaService;
        private readonly ISolicitudService _solicitudService;

        public UsuarioController(
            IUsuarioService usuarioService,
            IComputadorService computadorService,
            ISalaService salaService,
            ISolicitudService solicitudService)
        {
            _usuarioService = usuarioService;
            _computadorService = computadorService;
            _salaService = salaService;
            _solicitudService = solicitudService;
        }

        [HttpGet]
        public async Task<IActionResult> Principal()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerSalas()
        {
            var salas = await _salaService.GetSalas();
            return View("Salas", salas);
        }

        [HttpGet]
        public async Task<IActionResult> VerEquipos(Guid? salaId = null, string estado = "Todos")
        {
            var equipos = await _computadorService.GetComputadores();

            if (salaId.HasValue)
            {
                equipos = equipos.Where(e => e.SalaId == salaId.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(estado) && !estado.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                equipos = equipos.Where(e => string.Equals(e.Estado, estado, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var salas = await _salaService.GetSalas();
            var salaOptions = salas.Select(s => new { s.Id, Display = $"Sala {s.NumeroSalon}" }).ToList();
            ViewBag.Salas = new SelectList(salaOptions, "Id", "Display", salaId);
            ViewBag.Estado = estado;

            return View("Equipos", equipos);
        }

        [HttpGet]
        public async Task<IActionResult> VerSolicitudes()
        {
            var usuarioId = GetUsuarioIdActual();
            if (usuarioId == Guid.Empty) return Forbid();
            var solicitudes = await _solicitudService.GetSolicitudes();
            var mias = solicitudes.Where(s => s.UsuarioId == usuarioId).OrderByDescending(s => s.FechaInicio).ToList();
            var computadores = await _computadorService.GetComputadores();
            var salas = await _salaService.GetSalas();

            ViewBag.Computadores = computadores.ToDictionary(c => c.Id, c => c.Nombre);
            ViewBag.SalasDic = salas.ToDictionary(s => s.Id, s => $"Sala {s.NumeroSalon}");

            return View("Solicitudes", mias);
        }

        [HttpGet]
        public async Task<IActionResult> CrearSolicitud(string tipo = "Asignacion", Guid? computadorId = null, Guid? salaId = null)
        {
            var modelo = await BuildSolicitudModelo(tipo, salaId, computadorId);
            return View("CrearSolicitud", modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSolicitud(AñadirModeloSolicitud model, string tipo = "Asignacion")
        {
            var usuarioId = GetUsuarioIdActual();
            if (usuarioId == Guid.Empty) return Forbid();

            model.UsuarioId = usuarioId;
            model.Tipo = tipo;

            if (model.FechaFin < model.FechaInicio)
            {
                ModelState.AddModelError(nameof(model.FechaFin), "La fecha de fin debe ser mayor o igual a la de inicio.");
            }

             if (model.ComputadorId == Guid.Empty)
             {
                 ModelState.AddModelError(nameof(model.ComputadorId), "Debes seleccionar un equipo.");
             }

            if (!ModelState.IsValid)
            {
                await PopulateSolicitudCombos(tipo, model.SalaId, model.ComputadorId);
                ViewBag.TipoSolicitud = ObtenerTituloSolicitud(tipo);
                return View("CrearSolicitud", model);
            }

            await _solicitudService.AddSolicitud(model);
            TempData["Success"] = $"Solicitud de {ObtenerTituloSolicitud(tipo).ToLower()} enviada correctamente.";
            return RedirectToAction(nameof(VerSolicitudes));
        }

        [HttpGet]
        public async Task<IActionResult> ReportarIncidente(string tipo = "Danio", Guid? computadorId = null, Guid? salaId = null)
        {
            await PopulateSolicitudCombos(tipo, salaId, computadorId);
            ViewBag.TipoSolicitud = ObtenerTituloSolicitud(tipo);
            var model = new AñadirModeloSolicitud
            {
                UsuarioId = GetUsuarioIdActual(),
                Tipo = tipo,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddHours(1),
                SalaId = salaId,
                ComputadorId = computadorId ?? Guid.Empty
            };
            return View("ReportarIncidente", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportarIncidente(AñadirModeloSolicitud model, string tipo = "Danio")
        {
            var usuarioId = GetUsuarioIdActual();
            if (usuarioId == Guid.Empty) return Forbid();
            model.UsuarioId = usuarioId;
            model.Tipo = tipo;
            model.FechaInicio = DateTime.Now;
            model.FechaFin = DateTime.Now.AddHours(1);

            if (model.ComputadorId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(model.ComputadorId), "Debes seleccionar un equipo.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateSolicitudCombos(tipo, model.SalaId, model.ComputadorId);
                ViewBag.TipoSolicitud = ObtenerTituloSolicitud(tipo);
                return View("ReportarIncidente", model);
            }

            await _solicitudService.AddSolicitud(model);
            TempData["Success"] = $"El reporte de {ObtenerTituloSolicitud(tipo).ToLower()} fue enviado al coordinador.";
            return RedirectToAction(nameof(VerSolicitudes));
        }

        private async Task<AñadirModeloSolicitud> BuildSolicitudModelo(string tipo, Guid? salaId, Guid? computadorId)
        {
            await PopulateSolicitudCombos(tipo, salaId, computadorId);
            ViewBag.TipoSolicitud = ObtenerTituloSolicitud(tipo);
            return new AñadirModeloSolicitud
            {
                UsuarioId = GetUsuarioIdActual(),
                Tipo = tipo,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddHours(2),
                SalaId = salaId,
                ComputadorId = computadorId ?? Guid.Empty
            };
        }

        private async Task PopulateSolicitudCombos(string tipo, Guid? salaId, Guid? computadorId)
        {
            var salas = await _salaService.GetSalas();
            var salaOptions = salas.Select(s => new { s.Id, Display = $"Sala {s.NumeroSalon}" }).ToList();
            ViewBag.Salas = new SelectList(salaOptions, "Id", "Display", salaId);

            var computadores = await _computadorService.GetComputadores();
            if (tipo.Equals("Asignacion", StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals("Prestamo", StringComparison.OrdinalIgnoreCase))
            {
                computadores = computadores.Where(c => c.Estado.Equals("Disponible", StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var compOptions = computadores
                .Select(c => new
                {
                    c.Id,
                    Display = string.IsNullOrWhiteSpace(c.SalaDisplay) ? c.Nombre : $"{c.Nombre} ({c.SalaDisplay})"
                })
                .ToList();

            ViewBag.Computadores = new SelectList(compOptions, "Id", "Display", computadorId);
        }

        private Guid GetUsuarioIdActual()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
        }

        private static string ObtenerTituloSolicitud(string tipo) => tipo?.ToLowerInvariant() switch
        {
            "prestamo" => "Préstamo de equipo",
            "liberacion" => "Liberación de equipo",
            "danio" => "Reporte de daño",
            "asesoria" => "Asesoría técnica",
            _ => "Asignación de equipo"
        };
    }
}