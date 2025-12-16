using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Service.Services
{
    public class CarreraService : ICarreraService
    {
        private readonly AppDbContext _context;

        public CarreraService(AppDbContext context)
        {
            _context = context;
        }

        // CREAR CARRERA
        public async Task<(bool success, string? error)> CrearCarrera(
            string Nombre,
            string Sigla,
            int DuracionAnios,
            int ModalidadId,
            int TurnoId,
            string TituloOtorgado,
            int? PreceptorId)
        {
            // VALIDACIONES DE CAMPOS BÁSICOS

            // Validar campos obligatorios básicos
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Sigla) || DuracionAnios <= 0)
            {
                return (false, "Error: Nombre, Sigla y Duración en años son campos obligatorios.");
            }

            // Validar unicidad de la Sigla
            bool siglaYaExiste = await _context.Carreras
                .AsNoTracking()
                .AnyAsync(c => c.Sigla.ToUpper() == Sigla.ToUpper());

            if (siglaYaExiste)
            {
                return (false, $"Error: Ya existe una carrera con la Sigla '{Sigla.ToUpper()}'.");
            }

            // VALIDACIONES DE INTEGRIDAD REFERENCIAL (LLAVES FORÁNEAS)

            // Validar que la ModalidadId exista
            bool modalidadExiste = await _context.Modalidades
                .AsNoTracking()
                .AnyAsync(m => m.Id == ModalidadId);

            if (!modalidadExiste)
            {
                return (false, "Error: El ID de Modalidad proporcionado no es válido.");
            }

            // Validar que el TurnoId exista
            bool turnoExiste = await _context.Turnos
                .AsNoTracking()
                .AnyAsync(t => t.Id == TurnoId);

            if (!turnoExiste)
            {
                return (false, "Error: El ID de Turno proporcionado no es válido.");
            }

            if (PreceptorId != null)
            {
                // Validar que el PreceptorId exista
                bool preceptorConRolCorrecto = await _context.Usuarios
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == PreceptorId && u.RolId == 4);

                if (!preceptorConRolCorrecto)
                {
                    return (false, "Error: El Preceptor seleccionado no tiene el rol de Preceptor.");
                }
            }
            

            // CREACIÓN DEL OBJETO
            var nuevaCarrera = new Carrera
            {
                Nombre = Nombre,
                Sigla = Sigla.ToUpper(),
                DuracionAnios = DuracionAnios,
                TituloOtorgado = TituloOtorgado,
                ModalidadId = ModalidadId,
                TurnoId = TurnoId,
                PreceptorId = PreceptorId
            };

            // GUARDADO EN LA BASE DE DATOS
            try
            {
                _context.Carreras.Add(nuevaCarrera);
                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló el alta de la carrera por un conflicto de datos (revisar IDs de FK).");
            }
            catch (Exception)
            {
                return (false, "Error inesperado: Falló la creación de la Carrera.");
            }
        }

        // CONSULTAR CARRERA POR ID
        public async Task<Carrera?> GetCarreraByIdAsync(int CarreraId)
        {
            if (CarreraId <= 0) return null;

            return await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == CarreraId);
        }

        // CONSULTAR CARRERA ASOCIADA AL PRECEPTOR
        public async Task<LookupDTO?> GetCarreraIdByPreceptorIdAsync(int preceptorId)
        {
            // 1. Buscamos en la tabla de Carreras (Materias)
            return await _context.Carreras
                .AsNoTracking()
                .Where(c => c.PreceptorId == preceptorId)
                .Select(c => new LookupDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre
                })
                .FirstOrDefaultAsync();
        }

        // CONSULTAR TODAS LAS CARRERAS SIN PRECEPTOR MAS LA CARRERA DEL PRECEPTPR ENVIADO
        public async Task<ICollection<LookupDTO>> GetAllCarrerasSinPreceptorUPreceptorIDAsync(int PreceptorId)
        {
            return await _context.Carreras
                .AsNoTracking()
                .Where(c => c.PreceptorId == null || c.PreceptorId == PreceptorId)
                .OrderBy(c => c.Nombre)
                .Select(c => new LookupDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre
                })
                .ToListAsync();
        }

        // CONSULTAR TODAS LAS CARRERAS LISTA
        public async Task<ICollection<CarrerasListadoDTO>> GetAllCarrerasAsync()
        {
            var listaDetalles = _context.Carreras
                .AsNoTracking()
                .Include(c => c.Modalidad)
                .Include(c => c.Turno)
                .Include(c => c.Preceptor)
                .Select(c => new CarrerasListadoDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Sigla = c.Sigla,
                    DuracionAnios = c.DuracionAnios,
                    TituloOtorgado = c.TituloOtorgado,
                    Modalidad = c.Modalidad.Nombre,
                    Turno = c.Turno.Nombre,
                    Preceptor = c.Preceptor!.Persona.NombreCompleto != null ? c.Preceptor.Persona.NombreCompleto : "Sin Asignar"
                });

            return await listaDetalles.ToListAsync();
        }

        // CAONSULTAR TODAS LAS CARRERAS PARA DESPLEGABLE
        public async Task<ICollection<LookupDTO>> ObtenerTodasLasCarrerasAsync()
        {
            return await _context.Carreras
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new LookupDTO
            {
                Id = c.Id,
                Nombre = c.Nombre
            })
            .ToListAsync();
        }

        // CONSULTAR CARRERAS SIN PRECEPTOR
        public async Task<ICollection<LookupDTO>> ObtenerCarrerasSinPreceptorAsync()
        {
            return await _context.Carreras
                .AsNoTracking()
                .Where(c => c.PreceptorId == null)
                .OrderBy(c => c.Nombre)
                .Select(c => new LookupDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre
                })
                .ToListAsync();
        }

        // ASIGANAR PRECEPTOR A LA CARRERA
        public async Task AsignarPreceptorACarrera(int CarreraId, int PreceptorId)
        {
            Carrera? carreraExistente = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == CarreraId);

            if (carreraExistente != null && carreraExistente.PreceptorId == null)
            {
                carreraExistente.PreceptorId = PreceptorId;

                await _context.SaveChangesAsync();
            }
        }

        // ACTUALIZAR CARRERA
        public async Task<(bool success, string? error)> ActualizarCarreraAsync(
            int CarreraId,
            string? Nombre,
            int? DuracionAnios,
            int? ModalidadId,
            int? TurnoId,
            string? TituloOtorgado,
            int? PreceptorId)
        {
            // 1. Validaciones iniciales
            if (CarreraId <= 0)
            {
                return (false, "Error: El ID de la Carrera a buscar no es válido.");
            }

            Carrera? carreraExistente = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == CarreraId);

            if (carreraExistente == null)
            {
                return (false, "Error: La Carrera a actualizar no fue encontrada.");
            }

            // VALIDACIONES Y ASIGNACIONES CONDICIONALES
            bool actualizacion = false;

            // Modificar Nombre
            if (!string.IsNullOrWhiteSpace(Nombre) && carreraExistente.Nombre != Nombre)
            {
                carreraExistente.Nombre = Nombre;
                actualizacion = true;
            }

            // Modificar DuracionAnios
            if (DuracionAnios.HasValue && DuracionAnios.Value > 0 && carreraExistente.DuracionAnios != DuracionAnios.Value)
            {
                carreraExistente.DuracionAnios = DuracionAnios.Value;
                actualizacion = true;
            }

            // Modificar TituloOtorgado
            if (!string.IsNullOrWhiteSpace(TituloOtorgado) && carreraExistente.TituloOtorgado != TituloOtorgado)
            {
                carreraExistente.TituloOtorgado = TituloOtorgado;
                actualizacion = true;
            }

            // Modificar Modalidad
            if (ModalidadId.HasValue && carreraExistente.ModalidadId != ModalidadId.Value)
            {
                if (!await _context.Modalidades.AsNoTracking().AnyAsync(m => m.Id == ModalidadId.Value))
                {
                    return (false, "Error: El ID de Modalidad proporcionado no es válido.");
                }
                carreraExistente.ModalidadId = ModalidadId.Value;
                actualizacion = true;
            }

            // Modificar Turno
            if (TurnoId.HasValue && carreraExistente.TurnoId != TurnoId.Value)
            {
                if (!await _context.Turnos.AsNoTracking().AnyAsync(t => t.Id == TurnoId.Value))
                {
                    return (false, "Error: El ID de Turno proporcionado no es válido.");
                }
                carreraExistente.TurnoId = TurnoId.Value;
                actualizacion = true;
            }

            // F. Modificar Preceptor
            if (PreceptorId.HasValue && carreraExistente.PreceptorId != PreceptorId.Value)
            {
                bool preceptorConRolCorrecto = await _context.Usuarios
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == PreceptorId.Value && u.RolId == 4);

                if (!preceptorConRolCorrecto)
                {
                    return (false, "Error: El ID de Preceptor proporcionado no existe o no tiene el rol de Preceptor.");
                }
                carreraExistente.PreceptorId = PreceptorId.Value;
                actualizacion = true;
            }

            try
            {
                if (actualizacion)
                {
                    await _context.SaveChangesAsync();
                    return (true, null);
                }
                return (false, "Advertencia: La Carrera no fue modificada porque no se detectaron cambios en los datos proporcionados.");

            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló la actualización de la carrera por un conflicto de datos (ej. violación de clave foránea o datos inválidos).");
            }
            catch (Exception)
            {
                return (false, "Error inesperado: Falló la actualización de la Carrera.");
            }
        }

        // ELIMINAR CARRERA
        public async Task<(bool success, string? error)> EliminarCarreraAsync(int CarreraId)
        {
            Carrera? carreraAEliminar = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == CarreraId);

            if (carreraAEliminar == null)
            {
                return (false, $"Error: No se encontró una Carrera.");
            }

            // VALIDACIONES

            // VALIDACION DE INSCRIPCIONES
            bool hayInscripcionesAceptadas = await _context.Inscripciones
                .AsNoTracking()
                .AnyAsync(i => i.Materia.CarreraId == CarreraId && i.EstadoInscripcionId == 2);

            if (hayInscripcionesAceptadas)
            {
                return (false, "Error: No se puede eliminar la Carrera. Existen Alumnos con inscripciones ACEPTADAS en sus Materias.");
            }

            // VALIDACIÓN DE MATERIAS
            bool hayMateriasAsociadas = await _context.Materias
                .AsNoTracking()
                .AnyAsync(m => m.CarreraId == CarreraId);

            if (hayMateriasAsociadas)
            {
                return (false, "Error: La Carrera tiene Materias asociadas. Primero debe eliminar todas las Materias vinculadas.");
            }

            // VALIDACIÓN DE ALUMNOS INSCRIPTOS
            bool hayAlumnosDirectos = await _context.Alumnos
                .AsNoTracking()
                .AnyAsync(a => a.CarreraId == CarreraId);

            if (hayAlumnosDirectos)
            {
                return (false, "Error: No se puede eliminar la Carrera porque tiene Alumnos inscriptos directamente. Primero debe desvincularlos.");
            }

            // ELIMINACIÓN Y GUARDADO
            try
            {
                _context.Carreras.Remove(carreraAEliminar);
                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló la eliminación por una restricción de la base de datos.");
            }
            catch (Exception)
            {
                return (false, "Error inesperado al intentar eliminar la Carrera.");
            }
        }
    }
}
