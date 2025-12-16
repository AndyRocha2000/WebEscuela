using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.Cuenta
{
    public class CambiarContraseñaModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? SuccessMessage { get; set; }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        public int UsuarioId { get; set; }

        public CambiarContraseñaModel(
            IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "El campo {0} es obligatorio.")]
            [DataType(DataType.Password)]
            [Display(Name = "Nueva Contraseña")]
            public string Contraseña { get; set; } = string.Empty;

            [Required(ErrorMessage = "El campo {0} es obligatorio.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare(nameof(Contraseña), ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmarContraseña { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            // Verificacion si el usuario inicio sesion
            if (!User.Identity!.IsAuthenticated)
            {
                // Si por alguna razón no inision sesion lo enviamos al login
                return RedirectToPage("/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Obtener el ID del usuario de los Claims
            var userIdClaim = User.FindFirstValue("UserId");

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int usuarioId))
            {
                await HttpContext.SignOutAsync();
                return RedirectToPage("/Login", new { ErrorMessage = "Error de sesión. Por favor, inicie sesión nuevamente." });
            }

            // Validar el Modelo
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var (success, error) = await _usuarioService.CambiarContrasenia(
                usuarioId,
                Input.Contraseña);

            if (success)
            {
                SuccessMessage = "¡Contraseña actualizada con éxito! Puede continuar.";

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = error;
                return Page();
            }
        }
    }
}
