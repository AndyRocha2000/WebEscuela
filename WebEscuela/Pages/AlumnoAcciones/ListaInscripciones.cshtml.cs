using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.Services;

namespace WebEscuela.Pages.AlumnoAcciones
{
    public class ListaInscripcionesModel : PageModel
    {
        private readonly IInscripcionService _inscripcionService;
        private readonly IUsuarioService _usuarioService;

        public ListaInscripcionesModel(IInscripcionService inscripcionService, IUsuarioService usuarioService)
        {
            _inscripcionService = inscripcionService;
            _usuarioService = usuarioService;
        }

        public ICollection<InscripcionDetalleDto> Inscripciones { get; set; } = new List<InscripcionDetalleDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue("UserId");

            
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int parsedId))
            {
                return RedirectToPage("/Login");
            }

            // Verificamos la identidad
            var usuario = await _usuarioService.GetUsuarioByIdAsync(parsedId);
            if (usuario?.Persona is not Alumno)
            {
                TempData["ErrorMessage"] = "Solo los alumnos pueden acceder a esta sección.";
                return RedirectToPage("/Index");
            }

            // Obtenemos todas las inscripciones
            Inscripciones = await _inscripcionService.GetInscripcionesPorAlumnoAsync(int.Parse(userIdClaim));

            return Page();
        }
    }
}
