using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Pages.Gestion.Usuarios
{
    public class EditarUsuarioModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly ICarreraService _carreraService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? SuccessMessage { get; set; }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        public int UsuarioId { get; set; }

        public EditarUsuarioModel(
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
            var CarrerasSinPreceptorTask = _carreraService.GetAllCarrerasSinPreceptorUPreceptorIDAsync(UsuarioId);

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

            // Carreras disponibles para preceptor
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
            [EmailAddress(ErrorMessage = "El {0} no es válido.")]
            [Display(Name = "Correo Electrónico")]
            public string CorreoElectronico { get; set; } = string.Empty;

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
        public async Task<IActionResult> OnGetAsync(int usuarioId)
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
                TempData["ErrorMessage"] = "Acceso denegado: Solo los administradores pueden modificar usuarios.";
                return RedirectToPage("/Index");
            }

            // Verificacion de usuario valido para modificar
            if (usuarioId <= 0)
            {
                TempData["ErrorMessage"] = "Error: Se requiere un ID de usuario válido.";
                return RedirectToPage("./ListarUsuarios");
            }

            var usuarioAModificar = await _usuarioService.GetUsuarioByIdAsync(usuarioId);
            if (usuarioAModificar == null)
            {
                TempData["ErrorMessage"] = $"El usuario con ID {usuarioId} no existe.";
                return RedirectToPage("./ListarUsuarios");
            }

            // CARGA DE DATOS AL INPUT MODEL
            UsuarioId = usuarioId;
            Input.NombreCompleto = usuarioAModificar.Persona.NombreCompleto;
            Input.CorreoElectronico = usuarioAModificar.Persona.CorreoElectronico;
            Input.RolId = usuarioAModificar.RolId;

            // Carga datos especificos para Alumnos
            if (usuarioAModificar.Persona is Alumno alumno)
            {
                Input.CarreraId = alumno.CarreraId;
            }
            // Carga datos especificos para Docentes
            else if (usuarioAModificar.Persona is Docente docente)
            {
                Input.Titulo = docente.Titulo;
            }
            // Carga datos especificos para Preceptores
            else if (usuarioAModificar.RolId == 4)
            {
                var carreraActual = await _carreraService.GetCarreraIdByPreceptorIdAsync(usuarioId);
                Input.CarreraPreceptorId = carreraActual?.Id ?? 0;
            }

            await CargarDatosIniciales();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Revalidar campos básicos del modelo
            if (!ModelState.IsValid)
            {
                await CargarDatosIniciales();
                return Page();
            }

            // Ejecutar la lógica de actualización
            var (success, error) = await _usuarioService.ActualizarUsuarioAsync(
                UsuarioId,
                Input.NombreCompleto,
                Input.CorreoElectronico,
                null,
                Input.RolId,
                Input.CarreraId,
                Input.Titulo);

            if (success)
            {
                if (Input.CarreraPreceptorId.HasValue)
                {
                    await _carreraService.AsignarPreceptorACarrera(Input.CarreraPreceptorId.Value, UsuarioId);
                }

                SuccessMessage = $"Usuario {Input.NombreCompleto} creado exitosamente.";
                return RedirectToPage("./ListarUsuarios");
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
