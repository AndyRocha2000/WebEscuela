using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.Gestion.Materias
{
    public class EditarMateriaModel : PageModel
    {
        private readonly IMateriaService _materiaService;
        private readonly IUsuarioService _usuarioService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public int MateriaId { get; set; }

        [BindProperty]
        public int CarreraId { get; set; }

        [BindProperty]
        public string? CarreraNombre { get; set; }

        public EditarMateriaModel(
            IMateriaService materiaService, IUsuarioService usuarioService)
        {
            _materiaService = materiaService;
            _usuarioService = usuarioService;
        }

        private async Task CargarDatosIniciales()
        {
            var usuarios = await _usuarioService.ObtenerUsuarioPorRolAsync(3);

            Input.DocentesDisponibles = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Nombre
            }).ToList();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [StringLength(100)]
            public string? Nombre { get; set; } = string.Empty;

            [Required(ErrorMessage = "El Año es obligatorio.")]
            [Range(1, 6, ErrorMessage = "El año debe estar entre 1 y 6.")]
            public int? Anio { get; set; }

            [Required(ErrorMessage = "El Cuatrimestre es obligatorio.")]
            [Range(1, 2, ErrorMessage = "El cuatrimestre debe ser 1 o 2.")]
            public int? Cuatrimestre { get; set; }

            [Required(ErrorMessage = "El cupo máximo es obligatorio.")]
            [Range(1, 100, ErrorMessage = "El cupo debe ser entre 1 y 100.")]
            public int? CupoMaximo { get; set; }

            [Display(Name = "Docente")]
            public int? DocenteId { get; set; }

            public IEnumerable<SelectListItem> DocentesDisponibles { get; set; } = new List<SelectListItem>();
        }

        public async Task<IActionResult> OnGetAsync(int materiaId)
        {
            // Verificacion de Autenticacion
            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
            {
                return RedirectToPage("/Login");
            }

            // Verificacion de Rol Administrador
            var adminUsuario = await _usuarioService.GetUsuarioByIdAsync(adminId);
            if (adminUsuario == null || adminUsuario.RolId != 1)
            {
                TempData["ErrorMessage"] = "Acceso denegado: Solo administradores pueden modificar materias.";
                return RedirectToPage("/Index");
            }

            // Verificar si el id de la materia es un valor valido
            if (materiaId <= 0)
            {
                TempData["ErrorMessage"] = "Error: La página requiere un ID de materia válido.";
                return RedirectToPage("/Gestion/Carreras/ListarCarreras");
            }

            var materiaDto = await _materiaService.GetMateriaByIdAsync(materiaId);
            if (materiaDto == null)
            {
                TempData["ErrorMessage"] = $"Materia con ID {materiaId} no encontrada.";
                return RedirectToPage("/Gestion/Carreras/ListarCarreras");
            }

            // Cargar datos
            await ObtenerYAsignarContextoCarreraAsync(materiaId);
            MateriaId = materiaId;

            // Llenar inputs del formulario
            Input.Nombre = materiaDto.Nombre;
            Input.Anio = materiaDto.Anio;
            Input.Cuatrimestre = materiaDto.Cuatrimestre;
            Input.CupoMaximo = materiaDto.CupoMaximo;
            Input.DocenteId = materiaDto.DocenteId;

            await CargarDatosIniciales();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarDatosIniciales();
                return Page();
            }

            await ObtenerYAsignarContextoCarreraAsync(MateriaId);

            var (success, error) = await _materiaService.ActualizarMateria(
                MateriaId: MateriaId,
                Nombre: Input.Nombre,
                Anio: Input.Anio,
                Cuatrimestre: Input.Cuatrimestre,
                CupoMaximo: Input.CupoMaximo,
                DocenteId: Input.DocenteId
            );

            if (success)
            {
                TempData["SuccessMessage"] = $"La materia '{Input.Nombre}' fue actualizada exitosamente.";
                return RedirectToPage("./ListarMaterias", new { carreraId = CarreraId, carreraNombre = CarreraNombre });
            }
            else
            {
                ErrorMessage = error;
                await CargarDatosIniciales();

                return Page();
            }
        }

        private async Task<IActionResult?> ObtenerYAsignarContextoCarreraAsync(int materiaId)
        {
            var (carreraId, carreraNombre) = await _materiaService.GetCarreraContextByMateriaAsync(materiaId);

            if (carreraId <= 0)
            {
                TempData["ErrorMessage"] = "No se pudo encontrar la carrera asociada a la materia.";
                return RedirectToPage("/Gestion/Carreras/ListarCarreras");
            }

            CarreraId = carreraId;
            CarreraNombre = carreraNombre;

            return null;
        }
    }
}
