using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.Models.ModelosComputador;
using Services.Models.ModelosSala;
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
            // Para liberación, verificar que el usuario tenga un equipo asignado
            if (tipo.Equals("Liberacion", StringComparison.OrdinalIgnoreCase))
            {
                var usuarioId = GetUsuarioIdActual();
                var solicitudes = await _solicitudService.GetSolicitudes();
                var hoy = DateTime.Today;
                var miEquipoOcupado = solicitudes
                    .Where(s => s.UsuarioId == usuarioId &&
                               s.Estado == "Aceptado" &&
                               s.Tipo != "Liberacion" &&
                               s.FechaInicio.Date <= hoy &&
                               s.FechaFin.Date >= hoy)
                    .FirstOrDefault();
                
                if (miEquipoOcupado == null)
                {
                    TempData["Error"] = "No tienes ningún equipo asignado para liberar.";
                    return RedirectToAction(nameof(Principal));
                }
                
                // Prellenar sala y equipo del PC asignado
                salaId = miEquipoOcupado.SalaId;
                computadorId = miEquipoOcupado.ComputadorId;
            }
            else
            {
                // Si viene desde la vista de equipos, obtener salaId del computador
                if (computadorId.HasValue && !salaId.HasValue)
                {
                    var computador = await _computadorService.GetComputador(computadorId.Value);
                    if (computador != null && computador.SalaId.HasValue)
                    {
                        salaId = computador.SalaId;
                    }
                }
            }
            
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
            ViewBag.TipoSolicitud = ObtenerTituloSolicitud(tipo);
            ViewBag.Tipo = tipo;
            
            // NO prellenar nada para Danio y Asesoria - el usuario debe seleccionar primero la sala
            // Solo usar salaId si viene como parámetro (cuando el usuario selecciona una sala)
            
            await PopulateSolicitudCombos(tipo, salaId, computadorId);
            
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
            var ahora = DateTime.Now;
            // Redondear a minutos para evitar problemas con segundos
            ahora = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, 0);
            return new AñadirModeloSolicitud
            {
                UsuarioId = GetUsuarioIdActual(),
                Tipo = tipo,
                FechaInicio = ahora,
                FechaFin = ahora.AddHours(2),
                SalaId = salaId,
                ComputadorId = computadorId ?? Guid.Empty
            };
        }

        private async Task PopulateSolicitudCombos(string tipo, Guid? salaId, Guid? computadorId)
        {
            var salas = await _salaService.GetSalas();
            var usuarioId = GetUsuarioIdActual();
            var solicitudes = await _solicitudService.GetSolicitudes();
            var hoy = DateTime.Today;
            
            var computadores = await _computadorService.GetComputadores();
            
            // Obtener solicitudes activas para verificar disponibilidad real
            var solicitudesActivas = solicitudes
                .Where(s => s.Estado == "Aceptado" &&
                           s.FechaInicio.Date <= hoy &&
                           s.FechaFin.Date >= hoy)
                .Select(s => s.ComputadorId)
                .ToHashSet();
            
            if (tipo.Equals("Asignacion", StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals("Prestamo", StringComparison.OrdinalIgnoreCase))
            {
                // NO mostrar equipos hasta que se seleccione una sala
                if (salaId.HasValue)
                {
                    // Filtrar equipos disponibles (estado Disponible y no tienen solicitud activa) de la sala seleccionada
                    computadores = computadores
                        .Where(c => c.SalaId == salaId.Value &&
                                   c.Estado.Equals("Disponible", StringComparison.OrdinalIgnoreCase) &&
                                   !solicitudesActivas.Contains(c.Id))
                        .ToList();
                }
                else
                {
                    // Si no hay sala seleccionada, no mostrar equipos
                    computadores = new List<ModeloComputador>();
                }
            }
            else if (tipo.Equals("Liberacion", StringComparison.OrdinalIgnoreCase))
            {
                // Para liberación, mostrar solo los equipos que el usuario tiene asignados (ocupados por él)
                var misEquiposOcupados = solicitudes
                    .Where(s => s.UsuarioId == usuarioId &&
                               s.Estado == "Aceptado" &&
                               s.FechaInicio.Date <= hoy &&
                               s.FechaFin.Date >= hoy)
                    .Select(s => s.ComputadorId)
                    .ToHashSet();
                
                computadores = computadores
                    .Where(c => misEquiposOcupados.Contains(c.Id))
                    .ToList();
                
                // Si hay un computador seleccionado, asegurarse de que pertenece al usuario
                if (computadorId.HasValue)
                {
                    computadores = computadores
                        .Where(c => c.Id == computadorId.Value)
                        .ToList();
                }
            }
            else if (tipo.Equals("Danio", StringComparison.OrdinalIgnoreCase))
            {
                // Para reporte de daño: NO mostrar equipos al inicio, solo después de seleccionar sala
                // Excluir equipos bloqueados o en mantenimiento
                computadores = computadores
                    .Where(c => c.Estado != "Bloqueado" && c.Estado != "Mantenimiento")
                    .ToList();
                
                // Si hay una sala seleccionada, filtrar solo equipos de esa sala
                if (salaId.HasValue)
                {
                    computadores = computadores
                        .Where(c => c.SalaId == salaId.Value)
                        .ToList();
                }
                else
                {
                    // Si no hay sala seleccionada, no mostrar equipos
                    computadores = new List<ModeloComputador>();
                }
            }
            else if (tipo.Equals("Asesoria", StringComparison.OrdinalIgnoreCase))
            {
                // Para asesoría técnica: primero seleccionar sala, luego PCs de esa sala
                // Excluir equipos bloqueados o en mantenimiento
                computadores = computadores
                    .Where(c => c.Estado != "Bloqueado" && c.Estado != "Mantenimiento")
                    .ToList();
                
                // Si hay una sala seleccionada, filtrar solo equipos de esa sala
                if (salaId.HasValue)
                {
                    computadores = computadores
                        .Where(c => c.SalaId == salaId.Value)
                        .ToList();
                }
                else
                {
                    // Si no hay sala seleccionada, no mostrar equipos
                    computadores = new List<ModeloComputador>();
                }
            }

            // Para asignación y préstamo, solo mostrar salas disponibles (no en mantenimiento)
            if (tipo.Equals("Asignacion", StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals("Prestamo", StringComparison.OrdinalIgnoreCase))
            {
                salas = salas.Where(s => s.Estado == "Disponible").ToList();
            }
            
            // Crear SelectList de salas después de todos los filtros
            var salaOptions = salas.Select(s => new { s.Id, Display = $"Sala {s.NumeroSalon}" }).ToList();
            ViewBag.Salas = new SelectList(salaOptions, "Id", "Display", salaId);

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