using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.AlumnoAcciones
{
    public class CrearInscripcionModel : PageModel
    {
        private readonly IMateriaService _materiaService;
        private readonly IInscripcionService _inscripcionService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICarreraService _carreraService;

        public CrearInscripcionModel(
            IMateriaService materiaService,
            IInscripcionService inscripcionService,
            IUsuarioService usuarioService,
            ICarreraService carreraService)
        {
            _materiaService = materiaService;
            _inscripcionService = inscripcionService;
            _usuarioService = usuarioService;
            _carreraService = carreraService;
        }

        public List<MateriasListadoDTO> Materias { get; set; } = new();
        public string CarreraNombre { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            // Obtener ID de usuario de los Claims
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return RedirectToPage("/Login");

            // Obtener datos del Alumno
            var usuario = await _usuarioService.GetUsuarioByIdAsync(int.Parse(userId));
            if (usuario?.Persona is not Alumno alumno)
            {
                TempData["ErrorMessage"] = "Solo los alumnos pueden acceder a esta sección.";
                return RedirectToPage("/Index");
            }


            // Cargar materias de SU carrera
            var Carrera = await _carreraService.GetCarreraByIdAsync(alumno.CarreraId);
            CarreraNombre = Carrera!.Nombre ?? "Tu Carrera";
            Materias = (await _materiaService.GetAllMateriasByCarreraAsync(alumno.CarreraId)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int materiaId, int tipoCursadoId)
        {
            var userId = User.FindFirstValue("UserId");
            var usuario = await _usuarioService.GetUsuarioByIdAsync(int.Parse(userId!));

            string dniAlumno = usuario!.Persona.Dni;

            // CREAR LA INSCRIPCION
            var (success, error) = await _inscripcionService.CrearInscripcionAsync(dniAlumno, materiaId, tipoCursadoId);

            if (success)
            {
                TempData["SuccessMessage"] = "Inscripción registrada correctamente. Queda sujeta a aprobación del preceptor.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }

            return RedirectToPage();
        }
    }
}
