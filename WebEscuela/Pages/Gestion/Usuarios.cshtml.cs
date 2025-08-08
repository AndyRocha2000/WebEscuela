using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Service;

namespace WebEscuela.Pages.Gestion
{
    public class UsuariosModel : PageModel
    {
        private readonly GestionUsuarioService _servicio;

        public UsuariosModel(GestionUsuarioService servicio)
        {
            _servicio = servicio;
        }

        public List<Usuario> Usuarios { get; set; } = new();
        public List<Rol> RolesDisponibles { get; set; } = new();
        public List<Carrera> CarrerasDisponibles { get; set; } = new();


        public async Task OnGetAsync()
        {
            //Usuarios = await _servicio.ObtenerTodosAsync();
            RolesDisponibles = await _servicio.ObtenerRolesAsync();
            CarrerasDisponibles = await _servicio.ObtenerCarrerasAsync();
        }


        public async Task<IActionResult> OnPostEliminarAsync(int dni)
        {
            await _servicio.EliminarAsync(dni);
            return RedirectToPage();
        }
    }
}
