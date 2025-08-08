using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebEscuela.Repository.Models;
using WebEscuela.Repository.Repository;

namespace WebEscuela.Service.Service
{
    public class GestionUsuarioService
    {
        private readonly GestionUsuarioRepository _repositorio;
        private readonly GestionCarreraRepository _repositorioCarrera;

        public GestionUsuarioService(GestionUsuarioRepository repositorio, GestionCarreraRepository repositorioCarrera)
        {
            _repositorio = repositorio;
            _repositorioCarrera = repositorioCarrera;
        }

        // Lista de roles disponibles
        public async Task<List<Rol>> ObtenerRolesAsync()
        {
            return await _repositorio.ObtenerRolesAsync();
        }

        // Lista de carreras disponibles
        public Task<List<Carrera>> ObtenerCarrerasAsync()
        {
            return _repositorio.ObtenerCarrerasAsync();
        }


        // Obtener usuarios por DNI
        public Task<Persona?> ObtenerPersonaAsync(int dni)
        {
            return _repositorio.ObtenerPersonaPorDniAsync(dni);
        }
        public Task<Usuario?> ObtenerUsuarioAsync(int dni)
        {
            return _repositorio.ObtenerUsuarioPorDniAsync(dni);
        }







        // Crear usuario
        public async Task CrearUsuarioConPersonaAsync(string nombreCompleto, int dni, string correoElectronico, DateTime fechaNacimiento, int carreraId, string titulo, int rolId)
        {
            Persona persona;

            if (rolId == 2) // Alumno
            {
                persona = new Alumno
                {
                    NombreCompleto = nombreCompleto,
                    Dni = dni,
                    CorreoElectronico = correoElectronico,
                    FechaNacimiento = fechaNacimiento,
                    CarreraId = carreraId
                };
            }
            else if (rolId == 3) // Docente
            {
                persona = new Docente
                {
                    NombreCompleto = nombreCompleto,
                    Dni = dni,
                    CorreoElectronico = correoElectronico,
                    FechaNacimiento = fechaNacimiento,
                    Titulo = titulo
                };
            }
            else // Persona común
            {
                persona = new Persona
                {
                    NombreCompleto = nombreCompleto,
                    Dni = dni,
                    CorreoElectronico = correoElectronico,
                    FechaNacimiento = fechaNacimiento
                };
            }
            var personaCreada = await _repositorio.CrearPersonaAsync(persona);

            var usuario = new Usuario
            {
                CorreoElectronico = correoElectronico,
                Dni = dni,
                Contrasenia = dni.ToString(),
                RolId = rolId,
                PersonaId = personaCreada.Id
            };

            await _repositorio.CrearUsuarioAsync(usuario);
        }



        public Task EliminarAsync(int dni)
        {
            return _repositorio.EliminarPorDniAsync(dni);
        }


        // Función principal para editar Persona (Alumno, Docente o base)
        public async Task EditarPersonaAsync(Persona personaActualizada)
        {
            if (personaActualizada is Alumno alumnoActualizado)
            {
                await _repositorio.EditarAlumnoAsync(alumnoActualizado.Id, alumnoActualizado);
            }
            else if (personaActualizada is Docente docenteActualizado)
            {
                await _repositorio.EditarDocenteAsync(docenteActualizado.Id, docenteActualizado);
            }
            else
            {
                await _repositorio.EditarPersonaAsync(personaActualizada);
            }
        }


        public async Task CambiarRolUsuarioAsync(int dni, int nuevoRol, Dictionary<string, string> datosAdicionales)
        {
            // Validación según el nuevo rol
            switch (nuevoRol)
            {
                case 2: // Alumno
                    if (!datosAdicionales.ContainsKey("CarreraId"))
                        throw new Exception("Falta el campo 'CarreraId' para el rol Alumno.");

                    if (!int.TryParse(datosAdicionales["CarreraId"], out int carreraId))
                        throw new Exception("El valor de 'CarreraId' no es un número válido.");

                    var carreraExiste = await _repositorioCarrera.ObtenerPorIdAsync(int.Parse(datosAdicionales["Carrera"]));
                    if (carreraExiste == null)
                        throw new Exception("La carrera indicada no existe.");

                    break;

                case 3: // Docente
                    if (!datosAdicionales.ContainsKey("Titulo"))
                        throw new Exception("Falta el campo 'Titulo' para el rol Docente.");
                    break;

                    // Otros casos si tenés más roles especiales
            }

            // Si pasó las validaciones, se ejecuta el cambio
            await _repositorio.CambiarRolUsuarioAsync(dni, nuevoRol, datosAdicionales);
        }
    }
}
