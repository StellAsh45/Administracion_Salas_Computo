using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Models;
using Services;
using Services.Models.ModelosUsuario;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioService _usuarioService;

        public HomeController(ILogger<HomeController> logger, IUsuarioService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(string correo, string contrasena)
        {
            var user = await _usuarioService.GetByEmail(correo);
            if (user == null || user.Contrasena != contrasena)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña inválidos.");
                return View();
            }

            var claims = new List<Claim>
    {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Correo ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Correo ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Rol ?? string.Empty)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirecciones por rol
            if (!string.IsNullOrWhiteSpace(user.Rol) && user.Rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Principal", "Administrador");
            }

            if (!string.IsNullOrWhiteSpace(user.Rol) && user.Rol.Equals("Usuario", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Principal", "Usuario");
            }

            if (!string.IsNullOrWhiteSpace(user.Rol) &&
                user.Rol.IndexOf("Coordinador", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return RedirectToAction("Principal", "CoordinadorSala");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}