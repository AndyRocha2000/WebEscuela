using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Service.Services
{
    public class InscripcionService : IInscripcionService
    {
        private readonly AppDbContext _context;

        public InscripcionService(AppDbContext context)
        {
            _context = context;
        }

        // CREAR INSCRIPCION
        public async Task<(bool success, string? error)> CrearInscripcionAsync(
            string AlumnoDni,
            int MateriaId,
            int TipoCursadoId)
        {
            // VALIDACIONES

            // Validar Alumno Existe
            var alumno = await _context.Personas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Dni == AlumnoDni);

            if (alumno == null)
            {
                return (false, $"Error: No se encontró un alumno activo con DNI '{AlumnoDni}'.");
            }

            // Validar Materia Existe
            var materia = await _context.Materias
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == MateriaId);

            if (materia == null)
            {
                return (false, $"Error: No se encontró la materia con ID '{MateriaId}'.");
            }

            // Validar El tipo de Cursada
            if (TipoCursadoId <= 0 || TipoCursadoId > 2)
            {
                return (false, "Error: Tipo de Cursado inválido.");
            }

            // Validar que esta inscripto
            bool yaInscripto = await _context.Inscripciones
                .AsNoTracking()
                .AnyAsync(i => i.AlumnoId == alumno.Id && i.MateriaId == MateriaId && i.EstadoInscripcionId != 3);

            if (yaInscripto)
            {
                return (false, $"Error: El alumno ya tiene una inscripción activa o pendiente para la materia '{materia.Nombre}'.");
            }

            // Validar que aun queda cupo
            int cupoUsado = await _context.Inscripciones
                .CountAsync(i => i.MateriaId == MateriaId && i.EstadoInscripcionId != 3);

            if (cupoUsado >= materia.CupoMaximo)
            {
                return (false, "Error: La materia ha alcanzado su cupo máximo de alumnos.");
            }

            // CREAR INSCRIPCION
            var nuevaInscripcion = new Inscripcion
            {
                AlumnoId = alumno.Id,
                MateriaId = MateriaId,
                FechaInscripcion = DateTime.Now,
                EstadoInscripcionId = 1, // 1: Pendiente
                TipoCursadoId = TipoCursadoId
            };

            // GUARDAR EN LA BASE DE DATOS
            try
            {
                _context.Inscripciones.Add(nuevaInscripcion);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló el registro de la inscripción por un conflicto de datos en el servidor.");
            }
            catch (Exception)
            {
                return (false, "Error inesperado: Falló la creación de la Inscripción.");
            }
        }

        // CONSULTAR INSCRIPCIONES
        public async Task<ICollection<InscripcionDetalleDto>> GetInscripcionesAsync(
            int? MateriaId,
            int? EstadoId)
        {
            // CONSULTA BASE
            var query = _context.Inscripciones.AsNoTracking();

            // APLICACIÓN DE FILTROS CONDICIONALES
            if (MateriaId.HasValue && MateriaId.Value > 0)
            {
                query = query.Where(i => i.MateriaId == MateriaId.Value);
            }

            if (EstadoId.HasValue && EstadoId.Value > 0)
            {
                query = query.Where(i => i.EstadoInscripcionId == EstadoId.Value);
            }

            query = query.OrderByDescending(i => i.FechaInscripcion);

            var listaDetalles = await query
                .Select(i => new InscripcionDetalleDto
                {
                    // Clave Compuesta
                    AlumnoId = i.AlumnoId,
                    MateriaId = i.MateriaId,

                    // Datos de la Inscripción
                    FechaInscripcion = i.FechaInscripcion,
                    EstadoId = i.EstadoInscripcionId,
                    TipoCursado = i.TipoCursado.Nombre,

                    // Datos del Alumno
                    AlumnoDni = i.Alumno.Dni,
                    AlumnoNombre = i.Alumno.NombreCompleto,

                    // Datos de la Materia
                    MateriaNombre = i.Materia.Nombre,
                    DocenteAsignado = i.Materia.Docente!.NombreCompleto ?? "Sin Asignar"
                })
                .ToListAsync();

            return listaDetalles;
        }

        // LISTA DE INSCRIPCIONES DE ALUMNO
        public async Task<ICollection<InscripcionDetalleDto>> GetInscripcionesPorAlumnoAsync(int alumnoId)
        {
            return await _context.Inscripciones
                .AsNoTracking()
                .Where(i => i.AlumnoId == alumnoId)
                .OrderByDescending(i => i.FechaInscripcion)
                .Select(i => new InscripcionDetalleDto
                {
                    AlumnoId = i.AlumnoId,
                    MateriaId = i.MateriaId,
                    FechaInscripcion = i.FechaInscripcion,
                    EstadoId = i.EstadoInscripcionId,
                    TipoCursado = i.TipoCursado.Nombre,
                    MateriaNombre = i.Materia.Nombre,
                    DocenteAsignado = i.Materia.Docente!.NombreCompleto ?? "Sin Asignar",
                    CarreraNombre = i.Materia.Carrera.Nombre
                })
                .ToListAsync();
        }

        // ACTUALIZAR ESTADO DE INSCRIPCION
        public async Task<(bool success, string? error)> ActualizarEstadoAsync(
            int AlumnoId,
            int MateriaId,
            int NuevoEstadoId)
        {
            // VALIDACIONES

            // Validar datos iniciales
            if (AlumnoId <= 0 || MateriaId <= 0 || NuevoEstadoId <= 1 || NuevoEstadoId > 3)
            {
                return (false, "Error: IDs de Alumno/Materia o Nuevo Estado no válidos.");
            }

            // Buscar la Inscripcion
            var inscripcionExistente = await _context.Inscripciones
                .FirstOrDefaultAsync(i => i.AlumnoId == AlumnoId && i.MateriaId == MateriaId);

            if (inscripcionExistente == null)
            {
                return (false, "Error: No se encontró la inscripción para el alumno y materia especificados.");
            }

            // Valida que hubo un cambio de estado
            if (inscripcionExistente.EstadoInscripcionId == NuevoEstadoId)
            {
                return (false, "Advertencia: La inscripción ya se encuentra en el estado deseado.");
            }

            // Validar si queda Cupos disposibles
            if (NuevoEstadoId == 2)
            {
                // Buscar Materia
                var materia = await _context.Materias
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == MateriaId);

                if (materia == null) return (false, "Error interno: Materia no encontrada.");

                // Contar cuantos Inscripciones Aceotadas Existen
                int cuposYaOcupados = await _context.Inscripciones
                    .CountAsync(i => i.MateriaId == MateriaId && i.EstadoInscripcionId == 2);

                if (cuposYaOcupados >= materia.CupoMaximo)
                {
                    return (false, "Error: El cupo máximo de la materia ya ha sido alcanzado. No se puede aceptar esta inscripción.");
                }
            }

            // APLICAR CAMBIO DE ESTADO
            inscripcionExistente.EstadoInscripcionId = NuevoEstadoId;

            // GUARDAR EN LA BASE DE DATOS
            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló la actualización del estado de la inscripción por un conflicto de datos.");
            }
            catch (Exception)
            {
                return (false, "Error inesperado: Falló la actualización del estado.");
            }
        }

        // ELIMINAR INSCRIPCIÓN
        public (bool success, string? error) EliminarInscripcion(int AlumnoId, int MateriaId)
        {
            // Validar IDs
            if (AlumnoId <= 0 || MateriaId <= 0)
            {
                return (false, "Error: IDs de Alumno o Materia no válidos para la eliminación.");
            }

            // Buscar la inscripción por clave compuesta
            Inscripcion? inscripcionAEliminar = _context.Inscripciones
                .FirstOrDefault(i => i.AlumnoId == AlumnoId && i.MateriaId == MateriaId);

            if (inscripcionAEliminar == null)
            {
                return (false, "Error: No se encontró la inscripción para los IDs especificados.");
            }

            if (inscripcionAEliminar.EstadoInscripcionId == 2)
            {
                return (false, "Error: No se puede eliminar una inscripción que ha sido **ACEPTADA**. Considere cambiar el estado a 'Negada' o 'Cancelada' en su lugar.");
            }

            // 4. ELIMINACIÓN Y GUARDADO
            try
            {
                _context.Inscripciones.Remove(inscripcionAEliminar);
                _context.SaveChanges();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló la eliminación por una restricción de la base de datos.");
            }
            catch (Exception)
            {
                return (false, "Error inesperado al intentar eliminar la Inscripción.");
            }
        }
    }
}
