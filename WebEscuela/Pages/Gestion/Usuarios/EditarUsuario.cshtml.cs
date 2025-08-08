using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Service;

namespace WebEscuela.Pages.Gestion.Usuarios
{
    public class EditarUsuarioModel : PageModel
    {
        private readonly GestionUsuarioService _servicio;

        public EditarUsuarioModel(GestionUsuarioService servicio)
        {
            _servicio = servicio;
        }

        [BindProperty]
        public Persona Persona { get; set; } = new();

        [BindProperty]
        public Usuario Usuario { get; set; } = new();

        public List<Rol> RolesDisponibles { get; set; } = new();

        public bool MostrarAdvertencia { get; set; } = false;

        // Recibe DNI como parámetro para cargar la info
        public async Task<IActionResult> OnGetAsync(int dni)
        {
            RolesDisponibles = await _servicio.ObtenerRolesAsync();

            Persona? personaDb = await _servicio.ObtenerPersonaAsync(dni);
            Usuario? usuarioDb = await _servicio.ObtenerUsuarioAsync(dni);

            if (personaDb == null || usuarioDb == null)
            {
                return NotFound();
            }

            Persona = personaDb;
            Usuario = usuarioDb;

            // Aquí si querés, lógica para validar si puede cambiar rol o no
            // MostrarAdvertencia = await _servicio.TieneDatosRelacionadosAsync(dni);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            RolesDisponibles = await _servicio.ObtenerRolesAsync();
            
            await _servicio.EditarPersonaAsync(Persona);

            return RedirectToPage("/Gestion/Usuarios");
        }

    }
}
