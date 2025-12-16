using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.Services;

namespace WebEscuela.Pages.Gestion.Carreras
{
    public class ListarCarrerasModel : PageModel
    {
        private readonly ICarreraService _carreraService;
        private readonly IUsuarioService _usuarioService;

        public ICollection<CarrerasListadoDTO> Carreras { get; set; } = default!;

        public ListarCarrerasModel(ICarreraService carreraService, IUsuarioService usuarioService)
        {
            _carreraService = carreraService;
            _usuarioService = usuarioService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            //  Verificación de Autenticación
            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
            {
                return RedirectToPage("/Login");
            }

            // Verificación de Rol Administrador
            var adminUsuario = await _usuarioService.GetUsuarioByIdAsync(adminId);
            if (adminUsuario == null || adminUsuario.RolId != 1)
            {
                TempData["ErrorMessage"] = "Acceso denegado: Solo administradores pueden gestionar las carreras.";
                return RedirectToPage("/Index");
            }

            // Cargar datos
            Carreras = await _carreraService.GetAllCarrerasAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Error: Identificador de carrera no válido.";
                return RedirectToPage();
            }

            var (success, error) = await _carreraService.EliminarCarreraAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "La carrera ha sido eliminada exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }

            return RedirectToPage();
        }
    }
}
