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
        public async Task<IActionResult> BorrarUsuarioConfirmed(Guid id)
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

            await _salaService.AddSala(model);

            TempData["Success"] = "Sala registrada correctamente.";
            return RedirectToAction("VerSalas");
        }



        [HttpGet]
        public async Task<IActionResult> VerEquipos()
        {
            var equipos = await _computadorService.GetComputadores();
            equipos ??= new List<ModeloComputador>();

            return View("VerEquipos", equipos);
        }

        // REGISTRO GET
        [HttpGet]
        public async Task<IActionResult> RegistroEquipos()
        {
            var salas = await _salaService.GetSalas();

            ViewBag.Salas = salas.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.NumeroSalon.ToString()   // <-- ESTO ES LO QUE FALTABA
            }).ToList();

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

            await CargarSalas();

            return View("EditarEquipo", equipo);
        }

        // EDITAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarEquipo(ModeloComputador model)
        {
            if (!ModelState.IsValid)
            {
                await CargarSalas();
                return View("EditarEquipo", model);
            }

            await _computadorService.UpdateComputador(model);
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
        private async Task CargarSalas()
        {
            var salas = await _salaService.GetSalas();
            ViewBag.Salas = salas.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                //Text = s.NumeroSalon
            }).ToList();
        }

    }
}
