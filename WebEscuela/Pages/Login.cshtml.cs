using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Service;
using WebEscuela.Repository.Models;

namespace WebEscuela.Pages
{
    public class LoginModel : PageModel
    {
        private readonly InicioSesionService _servicio;

        public LoginModel(InicioSesionService servicio)
        {
            _servicio = servicio;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; }

        [BindProperty]
        [Required]
        public string Contrasenia { get; set; }

        public string MensajeError { get; set; }


        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var resultado = await _servicio.IniciarSesionAsync(CorreoElectronico, Contrasenia);

            if (!resultado.exito)
            {
                MensajeError = resultado.mensaje;
                return Page();
            }

            Usuario usuario = resultado.usuario!;
            string rol = resultado.rol!;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.CorreoElectronico),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Index");
        }

    }
}
