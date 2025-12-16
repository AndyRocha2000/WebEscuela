using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.Gestion.Carreras
{
    public class EditarCarreraModel : PageModel
    {
        private readonly ICarreraService _carreraService;
        private readonly IModalidadService _modalidadService;
        private readonly ITurnoService _turnoService;
        private readonly IUsuarioService _usuarioService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }


        [BindProperty]
        public int CarreraId { get; set; }

        public EditarCarreraModel(
            ICarreraService carreraService, IModalidadService modalidadService, ITurnoService turnoService, IUsuarioService usuarioService)
        {
            _carreraService = carreraService;
            _modalidadService = modalidadService;
            _turnoService = turnoService;
            _usuarioService = usuarioService;
        }

        private async Task CargarDatosIniciales()
        {
            var modalidadTask = _modalidadService.ObtenerTodasLasModalidadesAsync();
            var turnoTask = _turnoService.ObtenerTodosLosTurnosAsync();
            var preceptoresTask = _usuarioService.ObtenerPreceptoresSinCarreraAsync();

            // Esperar a que todas las tareas terminen
            await Task.WhenAll(modalidadTask, turnoTask, preceptoresTask);

            // Roles
            Input.ModalidadesDisponibles = modalidadTask.Result.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre
            }).ToList();

            // Carreras
            Input.TurnosDisponibles = turnoTask.Result.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Nombre
            }).ToList();

            // Materias
            Input.PreceptoresDisponibles = preceptoresTask.Result.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nombre
            }).ToList();
        }

        public class InputModel
        {
            // --- Campos de Persona ---
            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = string.Empty;

            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Range(1, 6, ErrorMessage = "La {0} debe ser un valor entre {1} y {2} años.")]
            [Display(Name = "Duracion")]
            public int Duracion { get; set; }

            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Display(Name = "TituloOtorgado")]
            public string TituloOtorgado { get; set; } = string.Empty;

            // --- Campos de Rol y Asignación Condicional ---
            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Display(Name = "Modalidad")]
            public int? ModalidadId { get; set; }

            [Required(ErrorMessage = "El {0} es obligatorio.")]
            [Display(Name = "Turno")]
            public int? TurnoId { get; set; }

            [Display(Name = "Preceptor")]
            public int? PreceptorId { get; set; }


            // --- Propiedades para los Dropdowns (NO se envían en el formulario) ---
            public IEnumerable<SelectListItem> ModalidadesDisponibles { get; set; } = Enumerable.Empty<SelectListItem>();
            public IEnumerable<SelectListItem> TurnosDisponibles { get; set; } = Enumerable.Empty<SelectListItem>();
            public IEnumerable<SelectListItem> PreceptoresDisponibles { get; set; } = Enumerable.Empty<SelectListItem>();
        }

        public async Task<IActionResult> OnGetAsync(int carreraId)
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
                TempData["ErrorMessage"] = "Acceso denegado: Solo administradores pueden modificar carreras.";
                return RedirectToPage("/Index");
            }

            // Verificar si el id de la carrera es un valor valido
            if (carreraId <= 0)
            {
                TempData["ErrorMessage"] = "Error: La página requiere un ID de carrera válido.";
                return RedirectToPage("./ListarCarreras");
            }

            var carreraAModificar = await _carreraService.GetCarreraByIdAsync(carreraId);
            if (carreraAModificar == null)
            {
                TempData["ErrorMessage"] = $"Carrera con ID {carreraId} no encontrada.";
                return RedirectToPage("./ListarCarreras");
            }

            // Cargar datos
            CarreraId = carreraId;

            // Llenar inputs del formulario
            Input.Nombre = carreraAModificar.Nombre;
            Input.Duracion = carreraAModificar.DuracionAnios;
            Input.TituloOtorgado = carreraAModificar.TituloOtorgado;

            Input.ModalidadId = carreraAModificar.ModalidadId;
            Input.TurnoId = carreraAModificar.TurnoId;
            Input.PreceptorId = carreraAModificar.PreceptorId;

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

            var (success, error) = await _carreraService.ActualizarCarreraAsync(
                CarreraId: CarreraId,
                Nombre: Input.Nombre,
                DuracionAnios: Input.Duracion,
                TituloOtorgado : Input.TituloOtorgado,
                ModalidadId: Input.ModalidadId,
                TurnoId: Input.TurnoId,
                PreceptorId: Input.PreceptorId
            );

            if ( success)
            {
                TempData["SuccessMessage"] = $"La carrera '{Input.Nombre}' fue actualizada exitosamente.";
                return RedirectToPage("./ListarCarreras");
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
