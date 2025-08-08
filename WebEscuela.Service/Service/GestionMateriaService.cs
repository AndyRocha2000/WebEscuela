using WebEscuela.Repository.Models;
using WebEscuela.Repository.Repository;

namespace WebEscuela.Service.Service
{
    public class GestionMateriaService
    {
        private readonly GestionMateriaRepository _repositorio;

        public GestionMateriaService(GestionMateriaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public Task<List<Materia>> ObtenerTodasAsync()
        {
            return _repositorio.ObtenerTodasAsync();
        }

        public Task CrearAsync(Materia materia)
        {
            return _repositorio.CrearAsync(materia);
        }

        public Task ActualizarAsync(Materia materia)
        {
            return _repositorio.ActualizarAsync(materia);
        }

        public Task EliminarAsync(int id)
        {
            return _repositorio.EliminarAsync(id);
        }
    }
}
