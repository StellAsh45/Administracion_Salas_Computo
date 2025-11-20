using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.Models.ModelosComputador;
using Services.Models.ModelosReporte;
using Services.Models.ModelosSolicitud;
using Services.Models.ModelosUsuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MvcSample.Controllers
{
    [Authorize(Roles = "Coordinador de Sala")]
    public class CoordinadorSalaController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IComputadorService _computadorService;
        private readonly ISolicitudService _solicitudService;
        private readonly ISalaService _salaService;
        private readonly IReporteService _reporteService;

        public CoordinadorSalaController(
            IUsuarioService usuarioService,
            IComputadorService computadorService,
            ISolicitudService solicitudService,
            ISalaService salaService,
            IReporteService reporteService)
        {
            _usuarioService = usuarioService;
            _computadorService = computadorService;
            _solicitudService = solicitudService;
            _salaService = salaService;
            _reporteService = reporteService;
        }

        [HttpGet]
        public async Task<IActionResult> Principal()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerEquipos()
        {
            var equipos = await _computadorService.GetComputadores();
            var solicitudes = await _solicitudService.GetSolicitudes();
            var usuarios = await _usuarioService.GetUsuarios();
            var hoy = DateTime.Today;
            
            // Obtener solicitudes activas (aceptadas y dentro del rango de fechas)
            // Excluir solicitudes de liberación que ya fueron aceptadas
            var solicitudesActivas = solicitudes
                .Where(s => s.Estado == "Aceptado" &&
                           s.Tipo != "Liberacion" &&
                           s.FechaInicio.Date <= hoy &&
                           s.FechaFin.Date >= hoy)
                .ToList();
            
            var usuariosDic = usuarios.ToDictionary(
                u => u.Id,
                u => string.IsNullOrWhiteSpace(u.Nombre) ? u.Correo : u.Nombre);
            
            // Asignar usuario que está usando cada equipo
            foreach (var equipo in equipos)
            {
                var solicitudActiva = solicitudesActivas
                    .FirstOrDefault(s => s.ComputadorId == equipo.Id);
                
                if (solicitudActiva != null && usuariosDic.TryGetValue(solicitudActiva.UsuarioId, out var nombreUsuario))
                {
                    equipo.UsuarioOcupando = nombreUsuario;
                }
            }
            
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];
            return View(equipos);
        }

        [HttpGet]
        public async Task<IActionResult> AsignarEquipo(Guid? salaId = null)
        {
            var vm = await BuildAsignarComputadorModel();
            if (salaId.HasValue)
            {
                vm.SalaId = salaId.Value;
                vm = await BuildAsignarComputadorModel(vm);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarEquipo(AsignarComputadorModel model)
        {
            if (model.FechaFin < model.FechaInicio)
            {
                ModelState.AddModelError(nameof(model.FechaFin), "La fecha fin debe ser mayor a la fecha de inicio.");
            }

            if (model.UsuarioId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(model.UsuarioId), "Debes seleccionar un usuario.");
            }

            if (model.ComputadorId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(model.ComputadorId), "Debes seleccionar un equipo.");
            }

            if (!ModelState.IsValid)
            {
                var vm = await BuildAsignarComputadorModel(model);
                return View(vm);
            }

            // Obtener el computador para obtener su SalaId
            var computador = await _computadorService.GetComputador(model.ComputadorId);
            var solicitud = new AñadirModeloSolicitud
            {
                UsuarioId = model.UsuarioId,
                ComputadorId = model.ComputadorId,
                SalaId = computador?.SalaId,
                FechaInicio = model.FechaInicio,
                FechaFin = model.FechaFin,
                Estado = "Aceptado"
            };

            await _solicitudService.AddSolicitud(solicitud);
            await _computadorService.SetEstado(model.ComputadorId, "Ocupado");

            TempData["Success"] = "Equipo asignado correctamente.";
            return RedirectToAction(nameof(VerEquipos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BloquearEquipo(Guid id)
        {
            // Cerrar cualquier solicitud activa que tenga este equipo ocupado
            await _solicitudService.CerrarSolicitudesActivasPorEquipo(id);
            
            await _computadorService.SetEstado(id, "Bloqueado");
            TempData["Success"] = "Equipo bloqueado y usuario removido.";
            return RedirectToAction(nameof(VerEquipos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LiberarEquipo(Guid id)
        {
            // Cerrar cualquier solicitud activa que tenga este equipo ocupado
            await _solicitudService.CerrarSolicitudesActivasPorEquipo(id);
            
            await _computadorService.SetEstado(id, "Disponible");
            TempData["Success"] = "Equipo liberado y usuario removido.";
            return RedirectToAction(nameof(VerEquipos));
        }

        [HttpGet]
        public async Task<IActionResult> Solicitudes(string estado = "Pendiente")
        {
            var solicitudes = await _solicitudService.GetByEstado(estado);
            var usuarios = await _usuarioService.GetUsuarios();
            var computadores = await _computadorService.GetComputadores();
            var salas = await _salaService.GetSalas();

            ViewBag.Usuarios = usuarios.ToDictionary(
                u => u.Id,
                u => string.IsNullOrWhiteSpace(u.Nombre) ? u.Correo : u.Nombre);

            ViewBag.Computadores = computadores.ToDictionary(
                c => c.Id,
                c => c.Nombre);

            ViewBag.SalasDic = salas.ToDictionary(
                s => s.Id,
                s => $"Sala {s.NumeroSalon}");

            ViewBag.Estado = estado;
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];
            return View(solicitudes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceptarSolicitud(Guid id)
        {
            await _solicitudService.AcceptSolicitud(id);
            TempData["Success"] = "Solicitud aceptada.";
            return RedirectToAction(nameof(Solicitudes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DenegarSolicitud(Guid id)
        {
            await _solicitudService.DenySolicitud(id);
            TempData["Success"] = "Solicitud denegada.";
            return RedirectToAction(nameof(Solicitudes));
        }

        [HttpGet]
        public async Task<IActionResult> OcupacionDiaria(DateTime? fecha)
        {
            var targetDate = fecha?.Date ?? DateTime.Today;
            var salas = await _salaService.GetSalas();
            var solicitudes = await _solicitudService.GetByEstado("Aceptado");
            var solicitudesActivas = solicitudes.Where(s =>
                s.FechaInicio.Date <= targetDate &&
                s.FechaFin.Date >= targetDate).ToList();

            var detalles = new List<OcupacionSalaDetalle>();

            foreach (var sala in salas)
            {
                var computadores = sala.Computadores ?? new List<ModeloComputador>();
                var idsComputadores = computadores.Select(c => c.Id).ToHashSet();
                
                // Equipos en uso (con solicitud aceptada activa)
                var equiposEnUso = solicitudesActivas
                    .Count(s => idsComputadores.Contains(s.ComputadorId));
                
                // Equipos ocupados (en mantenimiento o bloqueados)
                var equiposOcupados = computadores
                    .Count(c => c.Estado == "Mantenimiento" || c.Estado == "Bloqueado");
                
                // Equipos disponibles (disponibles y no en uso)
                var equiposDisponibles = computadores
                    .Count(c => c.Estado == "Disponible" && 
                               !solicitudesActivas.Any(s => s.ComputadorId == c.Id));

                detalles.Add(new OcupacionSalaDetalle
                {
                    SalaNombre = $"Sala {sala.NumeroSalon}",
                    Capacidad = sala.Capacidad,
                    EquiposRegistrados = computadores.Count,
                    EquiposDisponibles = equiposDisponibles,
                    EquiposEnUso = equiposEnUso,
                    EquiposOcupados = equiposOcupados
                });
            }

            var vm = new OcupacionDiariaModel
            {
                Fecha = targetDate,
                Detalles = detalles
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OcupacionSemanal(DateTime? inicioSemana)
        {
            var start = inicioSemana?.Date ?? GetStartOfWeek(DateTime.Today);
            var end = start.AddDays(6);

            var diasSemana = Enumerable.Range(0, 7)
                .Select(i => start.AddDays(i).Date)
                .ToList();

            var salas = await _salaService.GetSalas();
            var solicitudesAceptadas = await _solicitudService.GetByEstado("Aceptado");
            var solicitudesSemana = solicitudesAceptadas
                .Where(s => s.FechaInicio.Date <= end && s.FechaFin.Date >= start)
                .ToList();

            var vm = new OcupacionSemanalModel
            {
                InicioSemana = start,
                FinSemana = end,
                DiasSemana = diasSemana
            };

            foreach (var sala in salas)
            {
                var computadores = sala.Computadores ?? new List<ModeloComputador>();
                var idsSala = computadores.Select(c => c.Id).ToHashSet();

                var salaVm = new OcupacionSemanalSala
                {
                    SalaNombre = $"Sala {sala.NumeroSalon}",
                    EquiposRegistrados = computadores.Count
                };

                foreach (var dia in diasSemana)
                {
                    // Equipos en uso (con solicitud aceptada activa)
                    var equiposEnUso = solicitudesSemana.Count(s =>
                        s.FechaInicio.Date <= dia &&
                        s.FechaFin.Date >= dia &&
                        idsSala.Contains(s.ComputadorId));

                    salaVm.OcupacionPorDia[dia] = new OcupacionSemanalDia
                    {
                        EquiposRegistrados = computadores.Count,
                        EquiposOcupados = Math.Min(equiposEnUso, computadores.Count)
                    };
                }

                vm.Salas.Add(salaVm);
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ReportesEquipos()
        {
            var reportes = await _reporteService.GetByTipo("Equipos");
            var usuarios = await _usuarioService.GetUsuarios();
            ViewBag.Usuarios = usuarios.ToDictionary(
                u => u.Id,
                u => string.IsNullOrWhiteSpace(u.Nombre) ? u.Correo : u.Nombre);
            ViewBag.Success = TempData["Success"];
            return View(reportes.OrderByDescending(r => r.FechaGeneracion).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarReporteEquipos()
        {
            var equipos = await _computadorService.GetComputadores();
            var sb = new StringBuilder();
            sb.AppendLine("Estado actual de los equipos");
            foreach (var equipo in equipos)
            {
                sb.AppendLine($"{equipo.Nombre} - {equipo.Estado}");
            }

            await _reporteService.AddReporte(new AñadirModeloReporte
            {
                UsuarioId = GetUsuarioIdActual(),
                Tipo = "Equipos",
                Contenido = sb.ToString(),
                FechaGeneracion = DateTime.Now
            });

            TempData["Success"] = "Reporte de equipos generado.";
            return RedirectToAction(nameof(ReportesEquipos));
        }

        [HttpGet]
        public async Task<IActionResult> ReportesSalas()
        {
            var reportes = await _reporteService.GetByTipo("Salas");
            var usuarios = await _usuarioService.GetUsuarios();
            ViewBag.Usuarios = usuarios.ToDictionary(
                u => u.Id,
                u => string.IsNullOrWhiteSpace(u.Nombre) ? u.Correo : u.Nombre);
            ViewBag.Success = TempData["Success"];
            return View(reportes.OrderByDescending(r => r.FechaGeneracion).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarReporteSalas()
        {
            var salas = await _salaService.GetSalas();
            var sb = new StringBuilder();
            sb.AppendLine("Estado actual de las salas");
            foreach (var sala in salas)
            {
                var disponibles = sala.Computadores?.Count(c => c.Estado == "Disponible") ?? 0;
                var total = sala.Computadores?.Count ?? 0;
                sb.AppendLine($"Sala {sala.NumeroSalon} - {disponibles}/{total} equipos disponibles");
            }

            await _reporteService.AddReporte(new AñadirModeloReporte
            {
                UsuarioId = GetUsuarioIdActual(),
                Tipo = "Salas",
                Contenido = sb.ToString(),
                FechaGeneracion = DateTime.Now
            });

            TempData["Success"] = "Reporte de salas generado.";
            return RedirectToAction(nameof(ReportesSalas));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarReporteEquipos(Guid id)
        {
            await _reporteService.DeleteReporte(id);
            TempData["Success"] = "Reporte de equipos eliminado.";
            return RedirectToAction(nameof(ReportesEquipos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarReporteSalas(Guid id)
        {
            await _reporteService.DeleteReporte(id);
            TempData["Success"] = "Reporte de salas eliminado.";
            return RedirectToAction(nameof(ReportesSalas));
        }

        private async Task<AsignarComputadorModel> BuildAsignarComputadorModel(AsignarComputadorModel? current = null)
        {
            var usuarios = await _usuarioService.GetUsuarios();
            var computadores = await _computadorService.GetComputadores();
            var salas = await _salaService.GetSalas();

            var viewModel = current ?? new AsignarComputadorModel();

            viewModel.Usuarios = usuarios
                .Where(u => !string.IsNullOrWhiteSpace(u.Rol) &&
                            (u.Rol.Contains("Usuario", StringComparison.OrdinalIgnoreCase) ||
                             u.Rol.Contains("Investigador", StringComparison.OrdinalIgnoreCase)))
                .Select(u => new SelectOption
                {
                    Value = u.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(u.Nombre) ? u.Correo : u.Nombre
                })
                .ToList();

            // Filtrar solo salas disponibles
            var salasDisponibles = salas.Where(s => s.Estado == "Disponible").ToList();
            viewModel.Salas = salasDisponibles
                .Select(s => new SelectOption
                {
                    Value = s.Id.ToString(),
                    Text = $"Sala {s.NumeroSalon}"
                })
                .ToList();

            // Si hay una sala seleccionada, mostrar solo equipos de esa sala
            if (current?.SalaId.HasValue == true)
            {
                computadores = computadores
                    .Where(c => c.SalaId == current.SalaId.Value &&
                               string.Equals(c.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                computadores = computadores
                    .Where(c => string.Equals(c.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            viewModel.Computadores = computadores
                .Select(c => new SelectOption
                {
                    Value = c.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(c.SalaDisplay) ? c.Nombre : $"{c.Nombre} ({c.SalaDisplay})"
                })
                .ToList();

            return viewModel;
        }

        private Guid GetUsuarioIdActual()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}
