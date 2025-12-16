using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;

namespace WebEscuela.Service.Interfaces
{
    public interface IUsuarioService
    {
        // LOGIN
        Task<(Usuario? usuario, string? error)> Login(string contrasenia, string dni);

        // CAMBIAR CONTRASEÑA
        Task<(bool success, string? error)> CambiarContrasenia(int usuarioId, string nuevaContrasenia);

        // CONSULTA DE USUARIO POR ID
        Task<Usuario?> GetUsuarioByIdAsync(int usuarioId);

        // CONSULTA LISTA DE TODOS LOS USUARIOS
        Task<ICollection<UsuarioListadoDTO>> GetAllUsuarios();

        // CONSULTA DE PRECEPTORES SIN CARRERA
        Task<ICollection<LookupDTO>> ObtenerPreceptoresSinCarreraAsync();

        // CONSULTA DE USUARIO POR EL ROL
        Task<ICollection<LookupDTO>> ObtenerUsuarioPorRolAsync(int Id);

        // CREAR USUARIO/PERSONA
        Task<(bool success, string? error, int usuario)> CrearUsuarioAsync(
            string NombreCompleto,
            string DNI,
            string CorreoElectronico,
            DateTime FechaNacimiento,
            int rolId,
            int? CarreraId,
            string? Titulo,
            bool CambiarContraseña = true);

        // MODIFICAR USUARIO/PERSONA
        Task<(bool success, string? error)> ActualizarUsuarioAsync(
            int usuarioId,
            string? NombreCompleto,
            string? CorreoElectronico,
            string? Contrasenia, 
            int? RolID,
            int? CarreraId,
            string? Titulo);

        // ELIMINAR USUARIO/PERSONA
        Task<(bool success, string? error)> EliminarUsuarioAsync(int usuarioId);
    }
}
