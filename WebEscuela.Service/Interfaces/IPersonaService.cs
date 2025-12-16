using WebEscuela.Repository.Models;

namespace WebEscuela.Service.Interfaces
{
    public interface IPersonaService
    {
        // CREAR PERSONA
        Task<(bool success, string? error, Persona? persona)> CrearPersonaAsync(
            string NombreCompleto,
            string DNI,
            string CorreoElectronico,
            DateTime FechaNacimiento,
            int RolId,
            int? CarreraId,
            string? Titulo
        );

        // ACTUALIZAR DATOS PERSONALES
        (bool success, string? error) ActualizarDatosPersonales(
            int PersonaID,
            string NombreCompleto,
            string CorreoElectronico,
            DateTime FechaNacimiento
        );

        // CONSULTA PERSONA POR DNI
        Persona? GetPersonaByDni(string dni);
    }
}
