using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.Services;

namespace WebEscuela.Pages.PreceptorAcciones
{
    public class GestionarInscripcionesModel : PageModel
    {
        private readonly IInscripcionService _inscripcionService;
        private readonly IUsuarioService _usuarioService;

        public GestionarInscripcionesModel(IInscripcionService inscripcionService, IUsuarioService usuarioService)
        {
            _inscripcionService = inscripcionService;
            _usuarioService = usuarioService;
        }

        public ICollection<InscripcionDetalleDto> Inscripciones { get; set; } = new List<InscripcionDetalleDto>();
        public string MateriaNombre { get; set; } = string.Empty;
        public int MateriaId { get; set; }

        public async Task<IActionResult> OnGetAsync(int materiaId,string materiaNombre)
        {
            // Verificación de Sesión
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int preceptorId))
            {
                return RedirectToPage("/Login");
            }

            // Verificación de Rol
            var usuario = await _usuarioService.GetUsuarioByIdAsync(preceptorId);

            if (usuario == null || usuario.RolId != 4)
            {
                TempData["ErrorMessage"] = "Acceso denegado: Solo los preceptores pueden gestionar inscripciones.";
                return RedirectToPage("/Index");
            }

            // Validación de Parámetros de URL
            if (materiaId <= 0)
            {
                return RedirectToPage("./GestionInscripcionesMaterias");
            }

            // Carga de Datos
            MateriaId = materiaId;
            MateriaNombre = materiaNombre;

            // Obtenemos solo las inscripciones pendientes
            Inscripciones = await _inscripcionService.GetInscripcionesAsync(materiaId, 1);

            return Page();
        }

        public async Task<IActionResult> OnPostActualizarEstadoAsync(int alumnoId, int materiaId, int nuevoEstadoId)
        {
            var (success, error) = await _inscripcionService.ActualizarEstadoAsync(alumnoId, materiaId, nuevoEstadoId);

            if (success)
            {
                TempData["SuccessMessage"] = nuevoEstadoId == 2 ? "Inscripción Aceptada." : "Inscripción Rechazada.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }

            return RedirectToPage(new { materiaId = materiaId });
        }
    }
}
