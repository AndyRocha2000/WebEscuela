using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Service;

namespace WebEscuela.Pages.Gestion
{
    public class MateriasModel : PageModel
    {
        private readonly GestionMateriaService _servicio;

        public MateriasModel(GestionMateriaService servicio)
        {
            _servicio = servicio;
        }

        public List<Materia> Materias { get; set; } = new();

        public async Task OnGetAsync()
        {
            Materias = await _servicio.ObtenerTodasAsync();
        }

        public async Task<IActionResult> OnPostAgregarAsync(string Nombre, int Anio, int Cuatrimestre, int CupoMaximo, int CarreraId, int DocenteId)
        {
            var materia = new Materia
            {
                Nombre = Nombre,
                Anio = Anio,
                Cuatrimestre = Cuatrimestre,
                CupoMaximo = CupoMaximo,
                CarreraId = CarreraId,
                DocenteId = DocenteId
            };

            await _servicio.CrearAsync(materia);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditarAsync(int Id, string Nombre, int Anio, int Cuatrimestre, int CupoMaximo, int CarreraId, int DocenteId)
        {
            var materia = new Materia
            {
                Id = Id,
                Nombre = Nombre,
                Anio = Anio,
                Cuatrimestre = Cuatrimestre,
                CupoMaximo = CupoMaximo,
                CarreraId = CarreraId,
                DocenteId = DocenteId
            };

            await _servicio.ActualizarAsync(materia);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            await _servicio.EliminarAsync(id);
            return RedirectToPage();
        }
    }
}
