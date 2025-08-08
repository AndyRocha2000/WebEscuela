using WebEscuela.Repository.Models;
using WebEscuela.Repository.Repository;

namespace WebEscuela.Service.Service
{
    public class GestionCarreraService
    {
        private readonly GestionCarreraRepository _repositorio;

        public GestionCarreraService(GestionCarreraRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public Task<List<Carrera>> ObtenerTodasAsync()
        {
            return _repositorio.ObtenerTodasAsync();
        }

        public Task CrearAsync(Carrera carrera)
        {
            return _repositorio.CrearAsync(carrera);
        }

        public Task ActualizarAsync(Carrera carrera)
        {
            return _repositorio.ActualizarAsync(carrera);
        }

        public Task EliminarAsync(int id)
        {
            return _repositorio.EliminarAsync(id);
        }
    }
}
