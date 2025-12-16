using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.Gestion.Usuarios
{
    public class AgregarUsuarioModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly ICarreraService _carreraService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? SuccessMessage { get; set; }

        public string? ErrorMessage { get; set; }

        public AgregarUsuarioModel(
            IUsuarioService usuarioService,
            IRolService rolService,
            ICarreraService carreraService)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
            _carreraService = carreraService;
        }

        // --- Cargar Dropdowns ---
        private async Task CargarDatosIniciales()
        {
            var rolesTask = _rolService.GetAllRolesAsync();
            var carrerasTask = _carreraService.ObtenerTodasLasCarrerasAsync();
            var CarrerasSinPreceptorTask = _carreraService.ObtenerCarrerasSinPreceptorAsync();

            // Esperar a que todas las tareas terminen
            await Task.WhenAll(rolesTask, carrerasTask, CarrerasSinPreceptorTask);

            // Roles
            Input.RolesDisponibles = rolesTask.Result.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Nombre
            }).ToList();

            // Carreras
            Input.CarrerasDisponibles = carrerasTask.Result.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre
            }).ToList();

            // Carreras disponebles para preceptor
            Input.CarrerasPreceptorDisponibles = CarrerasSinPreceptorTask.Result.Select(cp => new SelectListItem
            {
                Value = cp.Id.ToString(),
                Text = cp.Nombre
            }).ToList();
        }

        public class InputModel
        {
            // --- Campos de Persona ---
            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Display(Name = "Nombre Completo")]
            public string NombreCompleto { get; set; } = string.Empty;

            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [StringLength(15, ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 7)]
            public string Dni { get; set; } = string.Empty;

            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [EmailAddress(ErrorMessage = "El {0} no es válido.")]
            [Display(Name = "Correo Electrónico")]
            public string CorreoElectronico { get; set; } = string.Empty;

            [Required(ErrorMessage = "La {0} es obligatoria.")]
            [Display(Name = "Fecha de Nacimiento")]
            [DataType(DataType.Date)]
            public DateTime FechaNacimiento { get; set; } = DateTime.Today;

            // --- Campos de Rol y Asignación Condicional ---
            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Display(Name = "Rol")]
            public int RolId { get; set; }

            // Campo para ALUMNO (Usado para la herencia)
            [Display(Name = "Carrera Asignada")]
            public int? CarreraId { get; set; }

            // Campo para PRECEPTOR ( Usado para darle una carrera al preceptor)
            [Display(Name = "Carrera")]
            public int? CarreraPreceptorId { get; set; }

            // Campo para DOCENTE (Usado para la herencia)
            [Display(Name = "Título Profesional")]
            public string? Titulo { get; set; }

            // --- Propiedades para los Dropdowns ---
            public IEnumerable<SelectListItem> RolesDisponibles { get; set; } = Enumerable.Empty<SelectListItem>();
            public IEnumerable<SelectListItem> CarrerasDisponibles { get; set; } = Enumerable.Empty<SelectListItem>();
            public IEnumerable<SelectListItem> CarrerasPreceptorDisponibles { get; set; } = Enumerable.Empty<SelectListItem>();
        }

        // --- GET ---
        public async Task<IActionResult> OnGet()
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


            // Carga datos iniciales
            await CargarDatosIniciales();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarDatosIniciales();

            // 1. Validación del Model
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validación de Campos Condicionales
            if (!ValidarAsignacionesCondicionales())
            {
                return Page();
            }

            // Crear usuario
            var (success, error, usuario) = await _usuarioService.CrearUsuarioAsync(
                Input.NombreCompleto,
                Input.Dni,
                Input.CorreoElectronico,
                Input.FechaNacimiento,
                Input.RolId,
                Input.CarreraId,
                Input.Titulo
            );

            if (success)
            {
                if (Input.CarreraPreceptorId.HasValue)
                {
                    await _carreraService.AsignarPreceptorACarrera(Input.CarreraPreceptorId.Value, usuario);
                }

                SuccessMessage = $"Usuario {Input.NombreCompleto} creado exitosamente.";
                return RedirectToPage("./ListarUsuarios");
            }
            else
            {
                // 5. Error: Manejar el error de Lógica de Negocio del servicio
                ErrorMessage = error;

                // Añadir el error al campo específico si es de DNI duplicado
                if (error != null && error.Contains("Ya existe una Persona registrada con este DNI"))
                {
                    ModelState.AddModelError("Input.Dni", "Ya existe una persona con este DNI.");
                }

                return Page();
            }
        }

        private bool ValidarAsignacionesCondicionales()
        {
            const int ALUMNO_ROL_ID = 2;
            const int DOCENTE_ROL_ID = 3;
            const int PRECEPTOR_ROL_ID = 4;

            // Validar que si selecciona Alumno, debe haber seleccionado una Carrera
            if (Input.RolId == ALUMNO_ROL_ID && Input.CarreraId == null)
            {
                ModelState.AddModelError("Input.CarreraId", "La carrera es obligatoria para este rol.");
                return false;
            }

            // Validar que si selecciona Docente, debe tener Título y Materias
            if (Input.RolId == DOCENTE_ROL_ID)
            {
                // Validación Título (Requerido para Herencia)
                if (string.IsNullOrWhiteSpace(Input.Titulo))
                {
                    ModelState.AddModelError("Input.Titulo", "El título profesional es obligatorio para el rol Docente.");
                    return false;
                }
            }

            // Limpieza de campos

            if (Input.RolId != ALUMNO_ROL_ID)
            {
                Input.CarreraId = null;
            }

            if (Input.RolId != PRECEPTOR_ROL_ID)
            {
                Input.CarreraPreceptorId = null;
            }

            if (Input.RolId != DOCENTE_ROL_ID)
            {
                Input.Titulo = null;
            }
            return true;
        }
    }
}
