using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEscuela.Repository.Models;
using WebEscuela.Service;
using WebEscuela.Service.Service;

namespace WebEscuela.Pages.Gestion
{
    public class CarrerasModel : PageModel
    {
        private readonly GestionCarreraService _servicio;

        public CarrerasModel(GestionCarreraService servicio)
        {
            _servicio = servicio;
        }

        public List<Carrera> Carreras { get; set; } = new();

        public async Task OnGetAsync()
        {
            Carreras = await _servicio.ObtenerTodasAsync();
        }

        public async Task<IActionResult> OnPostAgregarAsync(string Nombre, string Sigla, int DuracionAnios, string TituloOtorgado)
        {
            var carrera = new Carrera
            {
                Nombre = Nombre,
                Sigla = Sigla,
                DuracionAnios = DuracionAnios,
                TituloOtorgado = TituloOtorgado
            };

            await _servicio.CrearAsync(carrera);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditarAsync(int Id, string Nombre, string Sigla, int DuracionAnios, string TituloOtorgado)
        {
            var carrera = new Carrera
            {
                Id = Id,
                Nombre = Nombre,
                Sigla = Sigla,
                DuracionAnios = DuracionAnios,
                TituloOtorgado = TituloOtorgado
            };

            await _servicio.ActualizarAsync(carrera);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            await _servicio.EliminarAsync(id);
            return RedirectToPage();
        }
    }
}
