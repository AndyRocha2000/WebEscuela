using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;

namespace WebEscuela.Service.Interfaces
{
    public interface IMateriaService
    {
        // CREAR MATERIA
        Task<(bool success, string? error)> CrearMateriaASync(
            string Nombre,
            int Anio,
            int Cuatrimestre,
            int CupoMaximo, 
            int CarreraId,
            int? DocenteId
        );

        // CONSULTAR TODAS LAS MATERIAS
        Task<ICollection<LookupDTO>> GetAllMateriasAsync();

        // CONSULTAR MATERIA POR ID
        Task<Materia?> GetMateriaByIdAsync(int MateriaId);

        // CONSULTAR CARRERA POR ID MATERIA
        Task<(int CarreraId, string CarreraNombre)> GetCarreraContextByMateriaAsync(int materiaId);

        // CONSULTAR TODAS LAS MATERIAS DE UNA CARRERA
        Task<ICollection<MateriasListadoDTO>> GetAllMateriasByCarreraAsync(int Id);

        // CONSULTAR TODAS LAS MATERIAS A LAS CUALES UN ALUMNO SE PUEDE INSCRIBIR POR CARRERA
        Task<ICollection<MateriasListadoDTO>> GetMateriasDisponiblesByAlumnoAsync(int carreraId, int alumnoId);

        // CONSULTAR TODAS LAS MATERIAS SIN DOCENTE
        Task<ICollection<LookupDTO>> GetMateriasSinDocenteAsync();

        // ACTUALIZAR MATERIA
        Task<(bool success, string? error)> ActualizarMateria(
            int MateriaId,
            string? Nombre,
            int? Anio,
            int? Cuatrimestre,
            int? CupoMaximo,
            int? DocenteId
        );

        // ELIMINAR MATERIA
        Task<(bool success, string? error)> EliminarMateriaAsync(int MateriaId);
    }
}
