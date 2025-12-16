using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.Services;

namespace WebEscuela.Pages.Gestion.Usuarios
{
    public class ListarUsuariosModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;

        public ICollection<UsuarioListadoDTO> Usuarios { get; set; } = default!;

        public ListarUsuariosModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Verificación de Autenticación
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int adminId))
            {
                return RedirectToPage("/Login");
            }

            // Verificación de Rol Administrador
            var usuario = await _usuarioService.GetUsuarioByIdAsync(adminId);

            if (usuario == null || usuario.RolId != 1)
            {
                TempData["ErrorMessage"] = "Acceso denegado: Se requieren permisos de Administrador.";
                return RedirectToPage("/Index");
            }

            // Carga de la lista de Usuarios
            Usuarios = await _usuarioService.GetAllUsuarios();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Error: Identificador del usuario no válido.";
                return RedirectToPage();
            }

            var (success, error) = await _usuarioService.EliminarUsuarioAsync(id);

            Console.WriteLine(error);

            if (success)
            {
                TempData["SuccessMessage"] = "El usuario ha sido eliminada exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }

            return RedirectToPage();
        }
    }
}
