using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.Cuenta
{
    public class CambiarDatosPersonalesModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? SuccessMessage { get; set; }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        public int UsuarioId { get; set; }

        public CambiarDatosPersonalesModel(
            IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public class InputModel
        {
            // --- Campos de Persona ---
            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Display(Name = "Nombre Completo")]
            public string NombreCompleto { get; set; } = string.Empty;

            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [EmailAddress(ErrorMessage = "El {0} no es válido.")]
            [Display(Name = "Correo Electrónico")]
            public string CorreoElectronico { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Nueva Contraseña")]
            public string? Contraseña { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare(nameof(Contraseña), ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string? ConfirmarContraseña { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            // OBTENER Y PARSEAR EL UserId de los Claims de forma segura
            var userIdClaim = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
            {
                // En caso de sesión inválida, desloguear y redirigir
                await HttpContext.SignOutAsync();
                TempData["ErrorMessage"] = "Error de sesión. Por favor, inicie sesión nuevamente.";
                return RedirectToPage("/Login");
            }

            // 3. Obtener el usuario
            var usuarioAModificar = await _usuarioService.GetUsuarioByIdAsync(usuarioId);

            if (usuarioAModificar == null)
            {
                await HttpContext.SignOutAsync();
                TempData["ErrorMessage"] = "El usuario asociado a su sesión no fue encontrado.";
                return RedirectToPage("/Login");
            }

            // Llemar los Input
            Input.NombreCompleto = usuarioAModificar.Persona.NombreCompleto;
            Input.CorreoElectronico = usuarioAModificar.Persona.CorreoElectronico;

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

            string? contrasenia = string.IsNullOrWhiteSpace(Input.Contraseña) ? null : Input.Contraseña;

            var (success, error) = await _usuarioService.ActualizarUsuarioAsync(
                usuarioId,
                Input.NombreCompleto,
                Input.CorreoElectronico,
                contrasenia,
                null,
                null,
                null);

            if (success)
            {
                SuccessMessage = "¡Datos Personales actualizados con éxito! Puede continuar.";

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
