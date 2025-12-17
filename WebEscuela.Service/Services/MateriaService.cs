using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Service.Services
{
    public class MateriaService : IMateriaService
    {
        private readonly AppDbContext _context;

        public MateriaService(AppDbContext context)
        {
            _context = context;
        }

        // CREAR MATERIA
        public async Task<(bool success, string? error)> CrearMateriaASync(
            string Nombre,
            int Anio,
            int Cuatrimestre,
            int CupoMaximo,
            int CarreraId,
            int? DocenteId)
        {
            // VALIDACIONES DE CAMPOS BÁSICOS
            if (string.IsNullOrWhiteSpace(Nombre) || Anio <= 0 || Cuatrimestre <= 0 || CupoMaximo <= 0)
            {
                return (false, "Error: Nombre, Año, Cuatrimestre y Cupo Máximo deben ser valores válidos.");
            }

            bool carreraEncontrada = await _context.Carreras
                .AsNoTracking()
                .AnyAsync(c => c.Id == CarreraId);

            if (!carreraEncontrada)
            {
                return (false, $"Error: No se encontró una Carrera.");
            }

            
            if (DocenteId != null)
            {
                bool docenteConRolValido = await _context.Usuarios
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == DocenteId && u.RolId == 3);

                if (!docenteConRolValido)
                {
                    return (false, "Error: La persona con ese DNI no tiene un usuario asociado con el Rol de Docente activo.");
                }
            }


            // CREACIÓN DEL OBJETO
            var nuevaMateria = new Materia
            {
                Nombre = Nombre,
                Anio = Anio,
                Cuatrimestre = Cuatrimestre,
                CupoMaximo = CupoMaximo,
                CarreraId = CarreraId,
                DocenteId = DocenteId
            };

            // GUARDADO EN LA BASE DE DATOS
            try
            {
                _context.Materias.Add(nuevaMateria);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló el alta de la materia por un conflicto de datos.");
            }
            catch (Exception)
            {
                return (false, "Error inesperado: Falló la creación de la Materia.");
            }
        }

        // CONSULTAR TODAS LAS MATERIAS
        public async Task<ICollection<LookupDTO>> GetAllMateriasAsync()
        {
            return await _context.Materias
                .AsNoTracking()
                .OrderBy(m => m.Nombre)
                .Select(m => new LookupDTO
                {
                    Id = m.Id,
                    Nombre = $"{m.Nombre} ({m.Anio}° Año, Q{m.Cuatrimestre})"
                })
                .ToListAsync();
        }

        // CONSULTAR MATERIA POR ID
        public async Task<Materia?> GetMateriaByIdAsync(int MateriaId)
        {
            if (MateriaId <= 0) return null;

            return await _context.Materias
                .FirstOrDefaultAsync(m => m.Id == MateriaId);

        }

        // CONSULTAR CARRERA POR ID MATERIA
        public async Task<(int CarreraId, string CarreraNombre)> GetCarreraContextByMateriaAsync(int materiaId)
        {
            var carreraInfo = await _context.Materias
                .Where(m => m.Id == materiaId)
                .Select(m => new CarrerasListadoDTO
                {
                    Id = m.CarreraId,
                    Nombre = m.Carrera.Nombre
                })
                .FirstOrDefaultAsync();

            if (carreraInfo == null)
            {
                return (0, string.Empty);
            }

            return (carreraInfo.Id, carreraInfo.Nombre);
        }

        // CONSULTAR TODAS LAS MATERIAS DE UNA CARRERA
        public async Task<ICollection<MateriasListadoDTO>> GetAllMateriasByCarreraAsync(int CarreraId)
        {
            bool carreraEncontrada = await _context.Carreras
                .AsNoTracking()
                .AnyAsync(c => c.Id == CarreraId);

            if (!carreraEncontrada)
            {
                return new List<MateriasListadoDTO>();
            }

            // CONSULTA Y PROYECCIÓN
            var listaDetalles = _context.Materias
                .AsNoTracking()
                .Include(m => m.Docente)
                .Where(m => m.CarreraId == CarreraId)
                .OrderBy(m => m.Nombre)
                .Select(m => new MateriasListadoDTO
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Anio = m.Anio,
                    Cuatrimestre = m.Cuatrimestre,
                    CupoMaximo = m.CupoMaximo,
                    DocenteNombre = m.Docente!.NombreCompleto ?? "Sin Asignar"
                });

            return await listaDetalles.ToListAsync();
        }

        // CONSULTAR TODAS LAS MATERIAS A LAS CUALES UN ALUMNO SE PUEDE INSCRIBIR POR CARRERA
        public async Task<ICollection<MateriasListadoDTO>> GetMateriasDisponiblesByAlumnoAsync(int carreraId, int alumnoId)
        {
            // Identificar materias donde el alumno ya tiene una solicitud
            var materiasConCualquierRegistro = await _context.Inscripciones
                .Where(i => i.AlumnoId == alumnoId)
                .Select(i => i.MateriaId)
                .ToListAsync();

            
            var materiasDisponibles = await _context.Materias
                .AsNoTracking()
                .Include(m => m.Docente)
                .Include(m => m.Inscripciones)
                .Where(m => m.CarreraId == carreraId && !materiasConCualquierRegistro.Contains(m.Id))
                .Select(m => new MateriasListadoDTO
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Anio = m.Anio,
                    Cuatrimestre = m.Cuatrimestre,
                    CupoMaximo = m.CupoMaximo - m.Inscripciones.Count(i => i.EstadoInscripcionId != 3),
                    DocenteNombre = m.Docente != null ? m.Docente.NombreCompleto : "Sin Asignar"
                })
                // Solo se devuelven materias que al menos tengan un cupo
                .Where(dto => dto.CupoMaximo > 0)
                .ToListAsync();

            return materiasDisponibles;
        }


        // CONSULTAR TODAS LAS MATERIAS SIN DOCENTE
        public async Task<ICollection<LookupDTO>> GetMateriasSinDocenteAsync()
        {
            return await _context.Materias
                .AsNoTracking()
                .Where(m => m.DocenteId == null)
                .OrderBy(m => m.Nombre)
                .Select(m => new LookupDTO
                {
                    Id = m.Id,
                    Nombre = $"{m.Nombre} ({m.Anio}° Año, Q{m.Cuatrimestre})"
                })
                .ToListAsync();
        }

        // ACTUALIZAR MATERIA
        public async Task<(bool success, string? error)> ActualizarMateria(
            int MateriaId,
            string? Nombre,
            int? Anio,
            int? Cuatrimestre,
            int? CupoMaximo,
            int? DocenteId)
        {
            // Buscar la entidad existente
            if (MateriaId <= 0)
            {
                return (false, "Error: ID de Materia no válido para la actualización.");
            }

            Materia? materiaExistente = await _context.Materias.FirstOrDefaultAsync(m => m.Id == MateriaId);

            if (materiaExistente == null)
            {
                return (false, "Error: La Materia a actualizar no fue encontrada.");
            }

            // VALIDACIONES Y ASIGNACIONES CONDICIONALES
            bool actualizacion = false;

            // Modificar Nombre
            if (!string.IsNullOrWhiteSpace(Nombre) && materiaExistente.Nombre != Nombre)
            {
                materiaExistente.Nombre = Nombre;
                actualizacion = true;
            }

            // Modificar Año
            if (Anio.HasValue && Anio.Value > 0 && materiaExistente.Anio != Anio.Value)
            {
                materiaExistente.Anio = Anio.Value;
                actualizacion = true;
            }

            // Modificar Cuatrimestre
            if (Cuatrimestre.HasValue && Cuatrimestre.Value > 0 && materiaExistente.Cuatrimestre != Cuatrimestre.Value)
            {
                materiaExistente.Cuatrimestre = Cuatrimestre.Value;
                actualizacion = true;
            }

            // Modificar Cupo Máximo
            if (CupoMaximo.HasValue && CupoMaximo.Value > 0 && materiaExistente.CupoMaximo != CupoMaximo.Value)
            {
                int inscritosAceptados = await _context.Inscripciones
                    .AsNoTracking()
                    .CountAsync(i => i.MateriaId == MateriaId && i.EstadoInscripcionId == 2); // Asumiendo 2=Aceptada

                if (CupoMaximo.Value < inscritosAceptados)
                {
                    return (false, $"Error: El nuevo Cupo Máximo ({CupoMaximo.Value}) no puede ser menor al número de alumnos ya aceptados ({inscritosAceptados}).");
                }

                materiaExistente.CupoMaximo = CupoMaximo.Value;
                actualizacion = true;
            }

            // Modificar Docente (Validación de existencia y Rol de FK)
            if (DocenteId.HasValue && materiaExistente.DocenteId != DocenteId.Value)
            {
                // Asumimos que el Rol de Docente tiene ID = 3
                bool docenteValido = await _context.Usuarios
                    .AsNoTracking()
                    .AnyAsync(u => u.PersonaId == DocenteId.Value && u.RolId == 3);

                if (!docenteValido)
                {
                    return (false, "Error: El ID del Docente proporcionado no existe o no tiene el rol de Docente.");
                }
                materiaExistente.DocenteId = DocenteId.Value;
                actualizacion = true;
            }

            // GUARDADO
            try
            {
                if (actualizacion)
                {
                    await _context.SaveChangesAsync();
                    return (true, null);
                }
                return (false, "Advertencia: La Materia no fue modificada porque no se detectaron cambios.");
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló la actualización de la materia por un conflicto de datos (revisar integridad referencial).");
            }
            catch (Exception)
            {
                return (false, "Error inesperado: Falló la actualización de la Materia.");
            }
        }


        // ELIMINAR MATERIA
        public async Task<(bool success, string? error)> EliminarMateriaAsync(int MateriaId)
        {
            Materia? materiaAEliminar = await _context.Materias
                .FirstOrDefaultAsync(m => m.Id == MateriaId);

            if (materiaAEliminar == null)
            {
                return (false, "Error: No se encontró la Materia para eliminar.");
            }

            // VALIDACIÓN DE DEPENDENCIAS
            bool tieneInscripcionesAceptadas = await _context.Inscripciones
                .AsNoTracking()
                .AnyAsync(i => i.MateriaId == MateriaId && i.EstadoInscripcionId == 2);

            if (tieneInscripcionesAceptadas)
            {
                return (false, "Error: No se puede eliminar la Materia. Existen alumnos con inscripción ACEPTADA. Debe cambiar el estado o eliminar esas inscripciones primero.");
            }

            // ELIMINACIÓN Y GUARDADO
            try
            {
                _context.Materias.Remove(materiaAEliminar);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló la eliminación por una restricción de la base de datos (posiblemente una FK activa).");
            }
            catch (Exception)
            {
                return (false, "Error inesperado al intentar eliminar la Materia.");
            }
        }
    }
}
