using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Service;

namespace WebEscuela.Pages.Gestion.Usuarios
{
    public class AgregarUsuarioModel : PageModel
    {
        private readonly GestionUsuarioService _servicio;

        public AgregarUsuarioModel(GestionUsuarioService servicio)
        {
            _servicio = servicio;
        }

        public List<Rol> RolesDisponibles { get; set; } = new();
        public List<Carrera> CarrerasDisponibles { get; set; } = new();

        public async Task OnGetAsync()
        {
            RolesDisponibles = await _servicio.ObtenerRolesAsync();
            CarrerasDisponibles = await _servicio.ObtenerCarrerasAsync();
        }

        public async Task<IActionResult> OnPostAgregarAsync(
            string nombreCompleto,
            int dni,
            string correoElectronico,
            DateTime fechaNacimiento,
            int carreraId,
            string titulo,
            int rolId)
        {
            await _servicio.CrearUsuarioConPersonaAsync(
                nombreCompleto,
                dni,
                correoElectronico,
                fechaNacimiento,
                carreraId,
                titulo,
                rolId
            );

            return RedirectToPage("/Gestion/Usuarios"); // o donde quieras volver
        }
    }
}
