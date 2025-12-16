using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;

namespace WebEscuela.Service.Interfaces
{
    public interface ICarreraService
    {
        // CREAR CARRERA
        Task<(bool success, string? error)> CrearCarrera(
            string Nombre,
            string Sigla,
            int DuracionAnios,
            int ModalidadId,
            int TurnoId,
            string TituloOtorgado,
            int? PreceptorId
        );

        // CONSULTAR CARRERA POR ID
        Task<Carrera?> GetCarreraByIdAsync(int CarreraId);

        // CONSULTAR CARRERA ASOCIADA AL PRECEPTOR
        Task<LookupDTO?> GetCarreraIdByPreceptorIdAsync(int preceptorId);

        // CONSULTAR TODAS LAS CARRERAS SIN PRECEPTOR MAS LA CARRERA DEL PRECEPTPR ENVIADO
        Task<ICollection<LookupDTO>> GetAllCarrerasSinPreceptorUPreceptorIDAsync(int PreceptorId);

        // CONSULTAR TODAS LAS CARRERAS LISTA
        Task<ICollection<CarrerasListadoDTO>> GetAllCarrerasAsync();

        // CAONSULTAR TODAS LAS CARRERAS PARA DESPLEGABLE
        Task<ICollection<LookupDTO>> ObtenerTodasLasCarrerasAsync();

        // CONSULTAR CARRERAS SIN PRECEPTOR
        Task<ICollection<LookupDTO>> ObtenerCarrerasSinPreceptorAsync();

        // ASIGANAR PRECEPTOR A LA CARRERA
        Task AsignarPreceptorACarrera(int CarreraId, int PreceptorId);

        // ACTUALIZAR CARRERA
        Task<(bool success, string? error)> ActualizarCarreraAsync(
            int CarreraId,
            string? Nombre,
            int? DuracionAnios,
            int? ModalidadId,
            int? TurnoId,
            string? TituloOtorgado,
            int? PreceptorId
        );

        // ELIMINAR CARRERA
        Task<(bool success, string? error)> EliminarCarreraAsync(int Id);
    }
}
