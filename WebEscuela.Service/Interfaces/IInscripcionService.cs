using WebEscuela.Service.DTOs;

namespace WebEscuela.Service.Interfaces
{
    public interface IInscripcionService
    {
        // CREAR INSCRIPCION
        Task<(bool success, string? error)> CrearInscripcionAsync(
            string AlumnoDni,
            int MateriaNombre,
            int TipoCursadoId
        );

        // CONSULTAR INSCRIPCIONES
        Task<ICollection<InscripcionDetalleDto>> GetInscripcionesAsync(
            int? MateriaId = null,
            int? EstadoId = null
        );

        // LISTA DE INSCRIPCIONES DE ALUMNO
        Task<ICollection<InscripcionDetalleDto>> GetInscripcionesPorAlumnoAsync(int alumnoId);

        // ACTUALIZAR ESTADO DE INSCRIPCION
        Task<(bool success, string? error)> ActualizarEstadoAsync(
            int AlumnoId,
            int MateriaId,
            int NuevoEstadoId
        );

        // ELIMINAR INSCRIPCIÓN
        (bool success, string? error) EliminarInscripcion(int AlumnoId, int MateriaId);
    }
}
