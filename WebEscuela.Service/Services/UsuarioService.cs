using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;
using Microsoft.EntityFrameworkCore;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.DTOs;
using System.Globalization;

namespace WebEscuela.Service.Services
{
    // La clase hereda/implementa el contrato de la interfaz.
    public class UsuarioService : IUsuarioService
    {
        // Declaramos el contexto para poder hacer consultas
        private readonly AppDbContext _context;
        private readonly IPersonaService _personaService;

        // Constructor: Aquí recibimos el AppDbContext por inyección de dependencias
        public UsuarioService(AppDbContext context, IPersonaService personaService)
        {
            _context = context;
            _personaService = personaService;
        }

        // LOGIN
        public async Task<(Usuario? usuario, string? error)> Login(string contrasenia, string dni)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Persona)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Persona.Dni == dni);

            // 1. Fallo: DNI no existe
            if (usuario == null)
            {
                // Retorna un objeto Usuario nulo y el mensaje de error.
                return (null, "El DNI ingresado no está registrado.");
            }

            // 2. Fallo: Contraseña incorrecta
            if (usuario.Contrasenia != contrasenia)
            {
                // Retorna un objeto Usuario nulo y el mensaje de error.
                return (null, "Contraseña incorrecta.");
            }

            // 3. Éxito: Retorna el objeto Usuario y un mensaje de error nulo.
            return (usuario, null);
        }

        // CAMBIAR CONTRASEÑA
        public async Task<(bool success, string? error)> CambiarContrasenia(int usuarioId, string nuevaContrasenia)
        {
            // 1. VALIDACIÓN BÁSICA
            if (string.IsNullOrWhiteSpace(nuevaContrasenia))
            {
                return (false, "Error: La nueva contraseña no puede estar vacía.");
            }

            // 2. BUSCAR EL USUARIO
            var usuarioOriginal = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuarioOriginal == null)
            {
                return (false, "Error: El Usuario no fue encontrado.");
            }

            // 3. VALIDACIÓN DE NO CAMBIO
            if (usuarioOriginal.Contrasenia == nuevaContrasenia)
            {
                return (true, "Advertencia: La contraseña no fue modificada porque es idéntica a la actual.");
            }

            try
            {
                // 4. APLICAR CAMBIO
                usuarioOriginal.Contrasenia = nuevaContrasenia;
                usuarioOriginal.RequiereCambioContrasenia = false;

                // 5. GUARDAR CAMBIOS
                await _context.SaveChangesAsync();

                return (true, null); // Éxito
            }
            catch (DbUpdateException)
            {
                return (false, "Error de base de datos al intentar actualizar la contraseña.");
            }
            catch (Exception)
            {
                return (false, "Error inesperado al cambiar la contraseña.");
            }
        }

        // CONSULTA DE USUARIO POR ID
        public async Task<Usuario?> GetUsuarioByIdAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            return usuario;
        }

        // CONSULTA LISTA DE TODOS LOS USUARIOS
        public async Task<ICollection<UsuarioListadoDTO>> GetAllUsuarios()
        {
            var listaUsuarios = await _context.Usuarios
            .AsNoTracking()
            .Include(u => u.Persona)
            .Include(u => u.Rol)
            .Select(u => new UsuarioListadoDTO
            {
                Id = u.Id,
                NombreCompleto = u.Persona.NombreCompleto,
                Dni = u.Persona.Dni,
                Rol = u.Rol.Nombre,
                Email = u.Persona.CorreoElectronico,
                FechaNacimiento = u.Persona.FechaNacimiento
            })
            .ToListAsync();

            return listaUsuarios;
        }

        // CONSULTA DE PRECEPTORES SIN CARRERA
        public async Task<ICollection<LookupDTO>> ObtenerPreceptoresSinCarreraAsync()
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Persona)
                .Where(u => u.RolId == 4)
                .Where(u => !_context.Carreras.Any(c => c.PreceptorId == u.Id))
                .OrderBy(u => u.Persona.NombreCompleto)
                .Select(u => new LookupDTO
                {
                    Id = u.Id,
                    Nombre = u.Persona.NombreCompleto
                })
                .ToListAsync();
        }

        // CONSULTA DE USUARIO POR EL ROL
        public async Task<ICollection<LookupDTO>> ObtenerUsuarioPorRolAsync(int Id)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Persona)
                .Where(u => u.RolId == Id)
                .OrderBy(u => u.Persona.NombreCompleto)
                .Select(u => new LookupDTO
                {
                    Id = u.Id,
                    Nombre = u.Persona.NombreCompleto
                })
                .ToListAsync();
        }

        // CREAR USUARIO/PERSONA
        public async Task<(bool success, string? error, int usuario)> CrearUsuarioAsync(
            string NombreCompleto, 
            string DNI, 
            string CorreoElectronico, 
            DateTime FechaNacimiento, 
            int rolId,
            int? CarreraId,
            string? Titulo,
            bool CambiarContraseña = true)
        {
            // Validaciones de Rol

            bool rolExiste = await _context.Roles
                .AsNoTracking()
                .AnyAsync(r => r.Id == rolId);

            if (!rolExiste)
            {
                return (false, "Error de lógica de negocio: El Rol seleccionado no existe.", 0);
            }

            try
            {
                // CREAR LA PERSONA
                var (successPersona, errorPersona, personaGenerada) = await _personaService.CrearPersonaAsync(
                NombreCompleto, DNI, CorreoElectronico, FechaNacimiento, rolId, CarreraId, Titulo);

                if (!successPersona)
                {
                    return (false, errorPersona, 0);
                }

                int idNewUsuario = 0;

                if (personaGenerada != null)
                {
                    await _context.SaveChangesAsync();
                    // CREAR EL USUARIO
                    var nuevoUsuario = new Usuario
                    {
                        PersonaId = personaGenerada.Id,
                        Contrasenia = personaGenerada.Dni,
                        RolId = rolId,

                        
                    };

                    if (CambiarContraseña == false)
                    {
                        nuevoUsuario.RequiereCambioContrasenia = false;
                    }
                    else
                    {
                        nuevoUsuario.RequiereCambioContrasenia = true;
                    }

                    _context.Usuarios.Add(nuevoUsuario);
                    await _context.SaveChangesAsync();

                    idNewUsuario = nuevoUsuario.Id;
                }
                

                // 3. ÉXITO
                return (true, null, idNewUsuario);
            }
            catch (DbUpdateException)
            {
                return (false, "Error: Falló el alta del usuario por un conflicto de datos. Verifique los valores ingresados o intente nuevamente.", 0);
            }
            catch (Exception)
            {
                return (false, "Error inesperado: Falló la creación del Usuario.", 0);
            }
        }

        // MODIFICAR USUARIO/PERSONA
        public async Task<(bool success, string? error)> ActualizarUsuarioAsync(
            int usuarioId,
            string? NombreCompleto,
            string? CorreoElectronico,
            string? Contrasenia,
            int? RolID,
            int? CarreraId,
            string? Titulo)
        {
            var usuarioOriginal = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuarioOriginal == null || usuarioOriginal.Persona == null)
            {
                return (false, "Error: Usuario no encontrado.");
            }

            var personaOriginal = usuarioOriginal.Persona;

            bool cambioDeRolSolicitado = RolID.HasValue && usuarioOriginal.RolId != RolID.Value;

            // SI NO HAY CAMBIO DE ROL, ejecutamos la actualización parcial
            if (!cambioDeRolSolicitado)
            {
                bool actualizacion = false;

                // Si se proporciona un valor no nulo/vacío, actualiza.
                if (!string.IsNullOrWhiteSpace(NombreCompleto) && usuarioOriginal.Persona.NombreCompleto != NombreCompleto)
                {
                    usuarioOriginal.Persona.NombreCompleto = NombreCompleto;
                    actualizacion = true;
                }

                if (!string.IsNullOrWhiteSpace(CorreoElectronico) && usuarioOriginal.Persona.CorreoElectronico != CorreoElectronico)
                {
                    bool correoExiste = await _context.Personas
                        .AsNoTracking()
                        .AnyAsync(p => p.CorreoElectronico == CorreoElectronico && p.Id != usuarioId);

                    if (correoExiste)
                    {
                        return (false, "Error de validación: El Correo Electrónico ya está en uso por otro usuario.");
                    }

                    usuarioOriginal.Persona.CorreoElectronico = CorreoElectronico;
                    actualizacion = true;
                }

                if (!string.IsNullOrWhiteSpace(Contrasenia) && usuarioOriginal.Contrasenia != Contrasenia)
                {
                    usuarioOriginal.Contrasenia = Contrasenia;
                    actualizacion = true;
                }

                // --- 2. ACTUALIZACIÓN DE CAMPOS TPH ESPECÍFICOS ---

                if (usuarioOriginal.RolId == 2)
                {
                    if (usuarioOriginal.Persona is Alumno alumno && CarreraId.HasValue)
                    {
                        if (alumno.CarreraId != CarreraId.Value)
                        {
                            alumno.CarreraId = CarreraId.Value;
                            actualizacion = true;
                        }
                    }
                }
                else if (usuarioOriginal.RolId == 3)
                {
                    if (usuarioOriginal.Persona is Docente docente && !string.IsNullOrWhiteSpace(Titulo))
                    {
                        if (docente.Titulo != Titulo)
                        {
                            docente.Titulo = Titulo;
                            actualizacion = true;
                        }
                    }
                }

                // 3. Guardar cambios
                if (actualizacion)
                {
                    await _context.SaveChangesAsync();
                    return (true, null);
                }

                return (false, "Advertencia: El usuario no fue modificado porque no se detectaron cambios en los datos proporcionados.");

            }
            // SI HAY CAMBIO DE ROL, ejecutamos la actualización conpleta eliminando al Usuario y volviendolo a crear
            else
            {
                // Datos que no se pueden modificar
                string dniFinal = personaOriginal.Dni;
                DateTime fechaNacimientoFinal = personaOriginal.FechaNacimiento;

                // Datos que pueden ser modificados
                string nombreFinal = string.IsNullOrWhiteSpace(NombreCompleto) ? personaOriginal.NombreCompleto : NombreCompleto;
                string correoFinal = string.IsNullOrWhiteSpace(CorreoElectronico) ? personaOriginal.CorreoElectronico : CorreoElectronico;
                string contraseniaFinal = string.IsNullOrWhiteSpace(Contrasenia) ? usuarioOriginal.Contrasenia : Contrasenia;



                // EJECUCUIN DE transaction para protegernos en caso de que algo salga mal
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    int rolFinalId = RolID!.Value;

                    if (rolFinalId == 2)
                    {
                        if (!CarreraId.HasValue)
                        {
                            return (false, "Error de validación: El rol Alumno requiere especificar la CarreraId.");
                        }
                    }
                    else if (rolFinalId == 3)
                    {
                        if (string.IsNullOrWhiteSpace(Titulo))
                        {
                            return (false, "Error de validación: El rol Docente requiere especificar un Título.");
                        }
                    }


                    // ELIMINAR USUARIO/PERSONA ANTIGUO
                    var (successEliminacion, errorEliminacion) = await EliminarUsuarioAsync(usuarioId);

                    if (!successEliminacion)
                    {
                        await transaction.RollbackAsync();
                        return (false, errorEliminacion);
                    }

                    // CREAR NUEVO USUARIO/PERSONA
                    var (successCreacion, errorCreacion, newUsuarioId) = await CrearUsuarioAsync(
                        nombreFinal, 
                        dniFinal, 
                        correoFinal, 
                        fechaNacimientoFinal,
                        rolFinalId, 
                        CarreraId, 
                        Titulo,
                        false);

                    if (!successCreacion)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Error al re-crear el usuario: {errorCreacion}");
                    }

                    await transaction.CommitAsync();
                    return (true, null);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Error inesperado durante el cambio de rol: {ex.Message}");
                }
            }
        }

        // ELIMINAR USUARIO/PERSONA
        public async Task<(bool success, string? error)> EliminarUsuarioAsync(int usuarioId)
        {
            // Constantes de los Roles
            const int ALUMNO_ROL_ID = 2;
            const int DOCENTE_ROL_ID = 3;
            const int PRECEPTOR_ROL_ID = 4;
            const int ESTADO_ACEPTADA_ID = 2;

            try
            {
                // Buscar Usuario para obtener RolId y PersonaId
                var usuarioAEliminar = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == usuarioId);

                if (usuarioAEliminar == null)
                {
                    return (false, "Error: El Usuario con el ID proporcionado no existe.");
                }

                int personaId = usuarioAEliminar.PersonaId;
                int rolId = usuarioAEliminar.RolId;

                // VALIDACIONES EN FUNCION AL ROL

                // Rol ID 2: Alumno
                if (rolId == ALUMNO_ROL_ID)
                {
                    bool tieneInscripcionesAceptadas = await _context.Inscripciones
                        .AsNoTracking()
                        .AnyAsync(i => i.AlumnoId == personaId && i.EstadoInscripcionId == ESTADO_ACEPTADA_ID);

                    if (tieneInscripcionesAceptadas)
                    {
                        return (false, "Eliminación Cancelada: El Alumno tiene inscripciones 'Aceptadas'. Debe darlas de baja antes de eliminar el usuario.");
                    }
                }

                // Rol ID 3: Docente
                else if (rolId == DOCENTE_ROL_ID)
                {
                    bool tieneMateriasAsignadas = await _context.Materias
                        .AsNoTracking()
                        .AnyAsync(m => m.DocenteId == personaId);

                    if (tieneMateriasAsignadas)
                    {
                        return (false, "Eliminación Cancelada: El Docente aún tiene Materias a su cargo. Reasigne las materias antes de eliminarlo.");
                    }
                }

                // Rol ID 4: Preceptor
                else if (rolId == PRECEPTOR_ROL_ID)
                {
                    bool tieneCarrerasACargo = await _context.Carreras
                        .AsNoTracking()
                        .AnyAsync(c => c.PreceptorId == personaId);

                    if (tieneCarrerasACargo)
                    {
                        return (false, "Eliminación Cancelada: El Preceptor aún tiene Carreras a su cargo. Reasigne las carreras antes de eliminarlo.");
                    }
                }

                // ELIMINACION DEL USUARIO/PERSONA

                // Buscamos la Persona
                var personaAEliminar = await _context.Personas.FindAsync(personaId);

                if (personaAEliminar == null)
                {
                    return (false, "Error de integridad: La Persona asociada no fue encontrada.");
                }

                _context.Personas.Remove(personaAEliminar);

                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (DbUpdateException dbEx)
            {
                return (false, "Error de integridad de datos: Falló la eliminación. El usuario tiene datos relacionados que impiden su eliminación. " + dbEx.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado durante la eliminación del usuario: {ex.Message}");
            }
        }
    }
}