using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.Models.ModelosComputador;
using Services.Models.ModelosSala;
using Services.Models.ModelosUsuario;

namespace MvcSample.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministradorController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ISalaService _salaService;
        private readonly IComputadorService _computadorService;

        public AdministradorController(IUsuarioService usuarioService, ISalaService salaService, IComputadorService computadorService)
        {
            _usuarioService = usuarioService;
            _salaService = salaService;
            _computadorService = computadorService;
        }

        [HttpGet]
        public async Task<IActionResult> Principal()
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
                // Verificar si ya existe un administrador
                var usuarios = await _usuarioService.GetUsuarios();
                if (model.Rol == "Administrador" && usuarios.Any(u => u.Rol == "Administrador"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un administrador registrado. No se pueden registrar más administradores.");
                    return View("RegistroUsuarios", model);
                }

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
        public async Task<IActionResult> BorrarUsuario(Guid id)
        {
            await _usuarioService.DeleteUsuario(id);
            TempData["Success"] = "Usuario eliminado correctamente.";
            return RedirectToAction("VerUsuarios");
        }

        [HttpGet]
        public async Task<IActionResult> VerSalas()
        {
            var salas = await _salaService.GetSalas();
            salas ??= new List<ModeloSala>();
            return View("VerSalas", salas);
        }

        // REGISTRO SALAS - GET
        [HttpGet]
        public IActionResult RegistroSala()
        {
            return View("RegistroSala", new AñadirModeloSala());
        }

        // REGISTRO SALAS - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroSala(AñadirModeloSala model)
        {
            if (!ModelState.IsValid)
                return View("RegistroSala", model);

            try
            {
                await _salaService.AddSala(model);
                TempData["Success"] = "Sala registrada correctamente.";
                return RedirectToAction("VerSalas");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("RegistroSala", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar la sala.");
                return View("RegistroSala", model);
            }
        }

        // EDITAR SALAS - GET
        [HttpGet]
        public async Task<IActionResult> EditarSala(Guid id)
        {
            var sala = await _salaService.GetSala(id);
            if (sala == null) return NotFound();
            return View("EditarSala", sala);
        }

        // EDITAR SALAS - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarSala(ModeloSala model)
        {
            var existente = await _salaService.GetSala(model.Id);
            if (existente == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View("EditarSala", model);
            }
            existente.Capacidad = model.Capacidad;
            existente.Estado = model.Estado;

            await _salaService.UpdateSala(existente);
            TempData["Success"] = "Sala actualizada correctamente.";
            return RedirectToAction("VerSalas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrarSala(Guid id)
        {
            await _salaService.DeleteSala(id);
            TempData["Success"] = "Sala eliminada correctamente.";
            return RedirectToAction("VerSalas");
        }

        [HttpGet]
        public async Task<IActionResult> VerEquipos()
        {
            var equipos = await _computadorService.GetComputadores();
            equipos ??= new List<Services.Models.ModelosComputador.ModeloComputador>();

            var salas = await _salaService.GetSalas();
            salas ??= new List<Services.Models.ModelosSala.ModeloSala>();
            var salaDict = salas.ToDictionary(s => s.Id, s => s.NumeroSalon.ToString());

            foreach (var e in equipos)
            {
                if (!e.SalaId.HasValue || e.SalaId == Guid.Empty)
                {
                    e.SalaDisplay = "No asignada";
                }
                else if (salaDict.TryGetValue(e.SalaId.Value, out var salaText))
                {
                    e.SalaDisplay = salaText;
                }
                else
                {
                    e.SalaDisplay = "No asignada";
                }
            }

            return View("VerEquipos", equipos);
        }

        // REGISTRO GET
        [HttpGet]
        public async Task<IActionResult> RegistroEquipos()
        {
            await CargarSalas();
            return View(new AñadirModeloComputador());
        }


        // REGISTRO POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroEquipos(AñadirModeloComputador model)
        {
            if (!ModelState.IsValid)
            {
                await CargarSalas();
                return View(model);
            }

            // Validar capacidad de la sala
            if (model.SalaId.HasValue)
            {
                var sala = await _salaService.GetSala(model.SalaId.Value);
                if (sala != null)
                {
                    var equiposEnSala = await _salaService.GetComputadoresBySala(model.SalaId.Value);
                    if (equiposEnSala.Count >= sala.Capacidad)
                    {
                        ModelState.AddModelError(nameof(model.SalaId), $"La sala {sala.NumeroSalon} ya tiene el máximo de equipos permitidos ({sala.Capacidad}).");
                        await CargarSalas();
                        return View(model);
                    }
                }
            }

            await _computadorService.AddComputador(model);

            TempData["Success"] = "Equipo registrado correctamente.";
            return RedirectToAction("VerEquipos");
        }

        // EDITAR GET
        [HttpGet]
        public async Task<IActionResult> EditarEquipo(Guid id)
        {
            var equipo = await _computadorService.GetComputador(id);
            if (equipo == null) return NotFound();

            await CargarSalas(id, equipo.SalaId);

            return View("EditarEquipo", equipo);
        }

        // EDITAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarEquipo(ModeloComputador model)
        {
            var existente = await _computadorService.GetComputador(model.Id);
            if (existente == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarSalas(model.Id, existente.SalaId);
                return View("EditarEquipo", model);
            }

            // Validar capacidad de la sala si se está cambiando
            if (model.SalaId.HasValue && model.SalaId != existente.SalaId)
            {
                var sala = await _salaService.GetSala(model.SalaId.Value);
                if (sala != null)
                {
                    var equiposEnSala = await _salaService.GetComputadoresBySala(model.SalaId.Value);
                    if (equiposEnSala.Count >= sala.Capacidad)
                    {
                        ModelState.AddModelError(nameof(model.SalaId), $"La sala {sala.NumeroSalon} ya tiene el máximo de equipos permitidos ({sala.Capacidad}).");
                        await CargarSalas(model.Id, existente.SalaId);
                        return View("EditarEquipo", model);
                    }
                }
            }

            existente.Nombre = model.Nombre;
            existente.Estado = model.Estado;
            existente.SalaId = model.SalaId;

            await _computadorService.UpdateComputador(existente);
            TempData["Success"] = "Equipo actualizado correctamente.";
            return RedirectToAction("VerEquipos");
        }

        // ELIMINAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrarEquipo(Guid id)
        {
            await _computadorService.DeleteComputador(id);
            TempData["Success"] = "Equipo eliminado correctamente.";
            return RedirectToAction("VerEquipos");
        }

        // --------------------------------------
        // MÉTODO PARA LLENAR DROPDOWN
        // --------------------------------------
        private async Task CargarSalas(Guid? equipoId = null, Guid? salaActualId = null)
        {
            var salas = await _salaService.GetSalas();
            var salasDisponibles = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            
            foreach (var sala in salas)
            {
                var equiposEnSala = await _salaService.GetComputadoresBySala(sala.Id);
                // Si es edición y es la sala actual, siempre incluirla
                bool esSalaActual = salaActualId.HasValue && sala.Id == salaActualId.Value;
                // Si la sala no está llena o es la sala actual del equipo, incluirla
                if (equiposEnSala.Count < sala.Capacidad || esSalaActual)
                {
                    salasDisponibles.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = sala.Id.ToString(),
                        Text = $"Sala {sala.NumeroSalon} ({equiposEnSala.Count}/{sala.Capacidad})"
                    });
                }
            }
            
            ViewBag.Salas = salasDisponibles;
        }

    }
}
