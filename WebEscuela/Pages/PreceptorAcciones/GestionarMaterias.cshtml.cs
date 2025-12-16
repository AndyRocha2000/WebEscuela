using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.Services;

namespace WebEscuela.Pages.PreceptorAcciones
{
    public class GestionarMateriasModel : PageModel
    {
        private readonly IMateriaService _materiaService;
        private readonly ICarreraService _carreraService;
        private readonly IUsuarioService _usuarioService;

        public GestionarMateriasModel(IMateriaService materiaService, ICarreraService carreraService, IUsuarioService usuarioService)
        {
            _materiaService = materiaService;
            _carreraService = carreraService;
            _usuarioService = usuarioService;
        }

        public ICollection<MateriasListadoDTO> Materias { get; set; } = new List<MateriasListadoDTO>();
        public string CarreraNombre { get; set; } = "Mi Carrera";

        [BindProperty]
        public int CarreraId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Verificación de Autenticación
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int preceptorId))
            {
                return RedirectToPage("/Login");
            }

            // Verificacion de Rol
            var usuario = await _usuarioService.GetUsuarioByIdAsync(preceptorId);

            if (usuario == null || usuario.RolId != 4)
            {
                TempData["ErrorMessage"] = "Acceso denegado: No tienes permisos de Preceptor.";
                return RedirectToPage("/Index");
            }

            // Obtener la carrera que gestiona
            var carrera = await _carreraService.GetCarreraIdByPreceptorIdAsync(preceptorId);

            if (carrera == null)
            {
                TempData["ErrorMessage"] = "No tienes una carrera asignada para gestionar.";
                return RedirectToPage("/Index");
            }

            // Carga de datos para la vista
            CarreraNombre = carrera.Nombre;
            CarreraId = carrera.Id;

            Materias = await _materiaService.GetAllMateriasByCarreraAsync(carrera.Id);

            return Page();
        }

    }
}
