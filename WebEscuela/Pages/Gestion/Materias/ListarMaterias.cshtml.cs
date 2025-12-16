using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.Services;

namespace WebEscuela.Pages.Gestion.Materias
{
    public class ListarMateriasModel : PageModel
    {
        private readonly IMateriaService _materiaService;
        private readonly IUsuarioService _usuarioService;

        public ICollection<MateriasListadoDTO> Materias { get; set; } = new List<MateriasListadoDTO>();

        [BindProperty]
        public int CarreraId { get; set; }

        [BindProperty]
        public string? CarreraNombre { get; set; }

        // Constructor para inyección de dependencias
        public ListarMateriasModel(IMateriaService materiaService, IUsuarioService usuarioService)
        {
            _materiaService = materiaService;
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> OnGetAsync( int carreraId, string? carreraNombre)
        {
            // Verificación de Autenticación
            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
            {
                return RedirectToPage("/Login");
            }

            // Verificación de Rol Administrador
            var adminUsuario = await _usuarioService.GetUsuarioByIdAsync(adminId);
            if (adminUsuario == null || adminUsuario.RolId != 1)
            {
                TempData["ErrorMessage"] = "Acceso denegado: Se requieren permisos de administrador para gestionar materias.";
                return RedirectToPage("/Index");
            }

            // Verificar si la carrera tiene un valor valido
            if (carreraId <= 0)
            {
                TempData["ErrorMessage"] = "Debe proporcionar un ID de carrera válido.";
                return RedirectToPage("/Gestion/Carreras/ListarCarreras");
            }

            // Carga datos
            CarreraId = carreraId;
            CarreraNombre = carreraNombre;

            Materias = await _materiaService.GetAllMateriasByCarreraAsync(CarreraId);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int Materiaid, int carreraId, string? carreraNombre)
        {
            if (Materiaid <= 0)
            {
                TempData["ErrorMessage"] = "Error: Identificador de Materia no válido. " + Materiaid + " " + CarreraId + " " + CarreraNombre;
                return RedirectToPage("./ListarMaterias", new { carreraId = CarreraId, carreraNombre = CarreraNombre });
            }

            var (success, error) = await _materiaService.EliminarMateriaAsync(Materiaid);

            if (success)
            {
                TempData["SuccessMessage"] = "La Materia ha sido eliminada exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }
            return RedirectToPage("./ListarMaterias", new { carreraId = carreraId, carreraNombre = carreraNombre });
        }
    }
}
