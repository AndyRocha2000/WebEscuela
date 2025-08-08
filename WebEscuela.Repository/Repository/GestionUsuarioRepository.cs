using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;

namespace WebEscuela.Repository.Repository
{
    public class GestionUsuarioRepository
    {
        private readonly AppDbContext _context;

        public GestionUsuarioRepository(AppDbContext context)
        {
            _context = context;
        }


        // Obtener Roles Disponibles
        public async Task<List<Rol>> ObtenerRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        // Obtener Carreras Disponibles
        public async Task<List<Carrera>> ObtenerCarrerasAsync()
        {
            return await _context.Carreras.ToListAsync();
        }


        // Obtener todos los usuarios sin cargar Persona
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<List<Usuario>> ObtenerPorRolAsync(int rolId)
        {
            return await _context.Usuarios
                .Include(u => u.Persona)
                .Where(u => u.RolId == rolId)
                .ToListAsync();
        }


        // Obtener usuario por id sin cargar Persona
        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }


        // Agregar usuario
        public async Task<Persona> CrearPersonaAsync(Persona persona)
        {
            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();
            return persona;
        }
        public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }


        // Buscar Usuario y Persona por DNI
        public async Task<Persona?> ObtenerPersonaPorDniAsync(int dni)
        {
            return await _context.Personas.FirstOrDefaultAsync(p => p.Dni == dni);
        }

        public async Task<Usuario?> ObtenerUsuarioPorDniAsync(int dni)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Dni == dni);
        }


        // Eliminar usuario

        public async Task<bool> EliminarPorDniAsync(int dni)
        {
            var persona = await ObtenerPersonaPorDniAsync(dni);
            var usuario = await ObtenerUsuarioPorDniAsync(dni);

            if (usuario == null || persona == null)
            {
                return false;
            }

            _context.Usuarios.Remove(usuario);
            _context.Personas.Remove(persona);

            await _context.SaveChangesAsync();
            return true;
        }


        //////////////////////
        // Editar Persona
        public async Task EditarPersonaAsync(Persona personaActualizada)
        {
            var persona = await ObtenerPersonaPorDniAsync(personaActualizada.Dni);

            if (persona == null)
                throw new Exception("Persona no encontrada");

            bool seModifico = false;

            if (persona.NombreCompleto != personaActualizada.NombreCompleto)
            {
                persona.NombreCompleto = personaActualizada.NombreCompleto;
                seModifico = true;
            }

            if (persona.CorreoElectronico != personaActualizada.CorreoElectronico)
            {
                persona.CorreoElectronico = personaActualizada.CorreoElectronico;
                seModifico = true;

                // También actualiza el correo en Usuario
                var usuario = await ObtenerUsuarioPorDniAsync(personaActualizada.Dni);

                if (usuario != null)
                    usuario.CorreoElectronico = personaActualizada.CorreoElectronico;
            }

            if (seModifico)
                await _context.SaveChangesAsync();
        }

        // Editar alumno
        public async Task EditarAlumnoAsync(int personaId, Alumno alumnoActualizado)
        {
            var alumno = await _context.Personas.OfType<Alumno>()
                .FirstOrDefaultAsync(p => p.Id == personaId);

            if (alumno == null)
                throw new Exception("Alumno no encontrado");

            await EditarPersonaAsync(alumnoActualizado);

            if (alumno.CarreraId != alumnoActualizado.CarreraId)
                alumno.CarreraId = alumnoActualizado.CarreraId;

            await _context.SaveChangesAsync();
        }

        // Editar Docente
        public async Task EditarDocenteAsync(int personaId, Docente docenteActualizado)
        {
            var docente = await _context.Personas.OfType<Docente>()
                .FirstOrDefaultAsync(p => p.Id == personaId);

            if (docente == null)
                throw new Exception("Docente no encontrado");

            await EditarPersonaAsync(docenteActualizado);

            if (docente.Titulo != docenteActualizado.Titulo)
                docente.Titulo = docenteActualizado.Titulo;

            await _context.SaveChangesAsync();
        }




        //////////////////////

        public async Task CambiarRolUsuarioAsync(int dni, int nuevoRol, Dictionary<string, string> datosAdicionales)
        {
            var usuario = await ObtenerUsuarioPorDniAsync(dni);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var personaVieja = await ObtenerPersonaPorDniAsync(dni);
            if (personaVieja == null)
                throw new Exception("El usuario no tiene persona asociada");

            // Copiar datos base
            var Dni = personaVieja.Dni;
            var NombreCompleto = personaVieja.NombreCompleto;
            var Correo = personaVieja.CorreoElectronico;
            var fechaNacimiento = personaVieja.FechaNacimiento;

            Persona nuevaPersona;

            // Crear nueva persona según rol
            switch (nuevoRol)
            {
                case 2:
                    nuevaPersona = new Alumno
                    {
                        Dni = Dni,
                        NombreCompleto = NombreCompleto,
                        FechaNacimiento = fechaNacimiento,
                        CorreoElectronico = Correo,
                        CarreraId = int.Parse(datosAdicionales["Carrera"])
                    };
                    break;

                case 3:
                    nuevaPersona = new Docente
                    {
                        Dni = Dni,
                        NombreCompleto = NombreCompleto,
                        FechaNacimiento = fechaNacimiento,
                        CorreoElectronico = Correo,
                        Titulo = datosAdicionales["Titulo"]
                    };
                    break;

                default: // Persona base (por ejemplo, Invitado)
                    nuevaPersona = new Persona
                    {
                        Dni = Dni,
                        NombreCompleto = NombreCompleto,
                        FechaNacimiento = fechaNacimiento,
                        CorreoElectronico = Correo,
                    };
                    break;
            }

            // Guardar la nueva persona
            await CrearPersonaAsync(nuevaPersona);

            // Asignar la nueva persona al usuario
            usuario.PersonaId = nuevaPersona.Id;

            // Actualizar el rol
            usuario.RolId = nuevoRol;

            // Guardar cambios
            await _context.SaveChangesAsync();

            // Eliminar la persona anterior
            _context.Personas.Remove(personaVieja);
            await _context.SaveChangesAsync();
        }
    }
}
