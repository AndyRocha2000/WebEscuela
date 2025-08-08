using WebEscuela.Repository.Repository;
using WebEscuela.Repository.Models;

namespace WebEscuela.Service
{
    public class InicioSesionService
    {
        private readonly InicioSesionRepository _repositorio;

        public InicioSesionService(InicioSesionRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<(bool exito, string mensaje, Usuario? usuario, string? rol)> IniciarSesionAsync(string correo, string contrasenia)
        {
            var usuario = await _repositorio.ObtenerPorCorreoAsync(correo);

            if (usuario == null)
                return (false, "El correo no está registrado.", null, null);

            if (usuario.Contrasenia != contrasenia)
                return (false, "La contraseña es incorrecta.", null, null);

            var nombreRol = await _repositorio.ObtenerNombreRolAsync(usuario.RolId);

            return (true, "Inicio de sesión exitoso.", usuario, nombreRol);
        }
    }
}
