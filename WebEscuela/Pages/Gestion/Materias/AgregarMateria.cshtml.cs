using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.Gestion.Materias
{
    public class AgregarMateriaModel : PageModel
    {
        private readonly IMateriaService _materiaService;
        private readonly IUsuarioService _usuarioService;


        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public int CarreraId { get; set; }
        [BindProperty]
        public string? CarreraNombre { get; set; }

        public AgregarMateriaModel(
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
            public string Nombre { get; set; } = string.Empty;

            [Required(ErrorMessage = "El Año es obligatorio.")]
            [Range(1, 6, ErrorMessage = "El año debe estar entre 1 y 6.")]
            public int Anio { get; set; }

            [Required(ErrorMessage = "El Cuatrimestre es obligatorio.")]
            [Range(1, 2, ErrorMessage = "El cuatrimestre debe ser 1 o 2.")]
            public int Cuatrimestre { get; set; }

            [Required(ErrorMessage = "El cupo máximo es obligatorio.")]
            [Range(1, 100, ErrorMessage = "El cupo debe ser entre 1 y 100.")]
            public int CupoMaximo { get; set; }

            [Display(Name = "Docente")]
            public int? DocenteId { get; set; }

            public IEnumerable<SelectListItem> DocentesDisponibles { get; set; } = new List<SelectListItem>();
        }

        public async Task<IActionResult> OnGetAsync(int carreraId, string? carreraNombre)
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
                TempData["ErrorMessage"] = "Acceso denegado: Solo administradores pueden crear nuevas materias.";
                return RedirectToPage("/Index");
            }

            // Verificar si el id de la carrera es un valor valido
            if (carreraId <= 0)
            {
                TempData["ErrorMessage"] = "Error: La página requiere un ID de carrera válido para la creación de materias.";
                return RedirectToPage("/Gestion/Carreras/ListarCarreras");
            }

            // Cargar datos
            CarreraId = carreraId;
            CarreraNombre = carreraNombre;

            await CargarDatosIniciales();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            //  Validar el modelo
            if (!ModelState.IsValid)
            {
                await CargarDatosIniciales();
                return Page();
            }

            // Crear Materia
            var (success, error) = await _materiaService.CrearMateriaASync(
                Nombre :Input.Nombre,
                Anio : Input.Anio,
                Cuatrimestre :Input.Cuatrimestre,
                CupoMaximo :Input.CupoMaximo,
                CarreraId : CarreraId,
                DocenteId : Input.DocenteId
                );

            if (success)
            {
                TempData["SuccessMessage"] = $"La materia '{Input.Nombre}' fue creada exitosamente para la carrera {CarreraNombre}.";

                return RedirectToPage("./ListarMaterias", new { carreraId = CarreraId, carreraNombre = CarreraNombre });
            }
            else
            {
                ErrorMessage = error;
                await CargarDatosIniciales();
                return Page();
            }
        }
    }
}
