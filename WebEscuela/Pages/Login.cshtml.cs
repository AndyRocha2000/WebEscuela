using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Service;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;
        public string DniError { get; set; } = string.Empty;
        public string PasswordError { get; set; } = string.Empty;

        public LoginModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "El campo {0} es obligatorio.")]
            [Display(Name = "DNI de Usuario")]
            public string DNI { get; set; } = string.Empty;

            [Required(ErrorMessage = "El campo {0} es obligatorio.")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            ErrorMessage = string.Empty;

            DniError = string.Empty;

            PasswordError = string.Empty;
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var (user, error) = await _usuarioService.Login(Input.Password, Input.DNI);

            if (user != null)
            {
                // Creación de Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Persona.NombreCompleto),
                    new Claim(ClaimTypes.Role, user.Rol?.Nombre ?? "Invitado"),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Iniciar Sesion
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                if (user.RequiereCambioContrasenia == true)
                {
                    return RedirectToPage("/Cuenta/CambiarContraseña");
                }
                else
                {
                    return LocalRedirect(returnUrl);
                }
            }
            else
            {
                if (error == "El DNI ingresado no está registrado.")
                {
                    DniError = error;
                }
                else if (error == "Contraseña incorrecta.")
                {
                    PasswordError = error;
                }
                return Page();
            }
        }
    }
}
