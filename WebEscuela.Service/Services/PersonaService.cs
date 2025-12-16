using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Service.Services
{
    public class PersonaService : IPersonaService
    {
        // Declaramos el contexto para poder hacer consultas
        private readonly AppDbContext _context;

        // Constructor: Aquí recibimos el AppDbContext por inyección de dependencias
        public PersonaService(AppDbContext context)
        {
            _context = context;
        }

        // CREAR PERSONA
        public async Task<(bool success, string? error, Persona? persona)> CrearPersonaAsync(
            string NombreCompleto,
            string DNI,
            string CorreoElectronico,
            DateTime FechaNacimiento,
            int RolId,
            int? CarreraId,
            string? Titulo)
        {
            // Validacion del DNI
            bool dniYaExiste = await _context.Personas
                .AsNoTracking()
                .AnyAsync(p => p.Dni == DNI);

            if (dniYaExiste)
            {
                return (false, "Error: Ya existe una Persona registrada con este DNI.", null);
            }
            else if (string.IsNullOrWhiteSpace(DNI))
            {
                return (false, "Error: El DNI es un campo obligatorio.", null);
            }

            // Validacion del Nombre Completo
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                return (false, "Error: El Nombre Completo es un campo obligatorio.", null);
            }

            // Validacion del CorreoElectronico
            bool CorreoYaExiste = await _context.Personas
                .AsNoTracking()
                .AnyAsync(p => p.CorreoElectronico == CorreoElectronico);

            if (CorreoYaExiste)
            {
                return (false, "Error: Ya existe una Persona registrada con este CorreoElectronico.", null);
            }
            else if (string.IsNullOrWhiteSpace(CorreoElectronico))
            {
                return (false, "Error: El Nombre CorreoElectronico es un campo obligatorio.", null);
            }

            // Validacion del Fecha de Nacimiento
            if (FechaNacimiento.AddYears(18) > DateTime.Today)
            {
                return (false, "Error: La persona debe tener al menos 18 años.", null);
            }

            // Rol 2: ALUMNO Validacion CarreraId
            if (RolId == 2)
            {
                if (!CarreraId.HasValue || CarreraId.Value <= 0)
                {
                    return (false, "Error: El ID de la Carrera es obligatorio para el rol Alumno.", null);
                }
                // Verificar que la Carrera exista
                if (!await _context.Carreras.AsNoTracking().AnyAsync(c => c.Id == CarreraId.Value))
                {
                    return (false, "Error: La Carrera seleccionada para el Alumno no existe.", null);
                }
            }

            // Rol 3: DOCENTE Validacion Titulo
            if (RolId == 3)
            {
                if (string.IsNullOrWhiteSpace(Titulo))
                {
                    return (false, "Error: El título es obligatorio para el rol Docente.", null);
                }
            }

            // Crear a la Persona
            Persona nuevaPersona;

            if (RolId == 2) // Alumno
            {
                var alumno = new Alumno();
                alumno.CarreraId = CarreraId!.Value;
                nuevaPersona = alumno;
            }
            else if (RolId == 3) // Docente
            {
                var docente = new Docente();
                docente.Titulo = Titulo!;
                nuevaPersona = docente;
            }
            else
            {
                nuevaPersona = new Persona();
            }

            // Asignar propiedades comunes
            nuevaPersona.NombreCompleto = NombreCompleto;
            nuevaPersona.Dni = DNI;
            nuevaPersona.CorreoElectronico = CorreoElectronico;
            nuevaPersona.FechaNacimiento = FechaNacimiento;

            _context.Personas.Add(nuevaPersona);

            return (true, null, nuevaPersona);
        }

        // ACTUALIZAR DATOS PERSONALES
        public (bool success, string? error) ActualizarDatosPersonales(
            int PersonaID,
            string NombreCompleto,
            string CorreoElectronico,
            DateTime FechaNacimiento)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                return (false, "Error: El nombre completo no puede estar vacío.");
            }

            // Cargar el objeto original
            var personaOriginal = _context.Personas
                .FirstOrDefault(p => p.Id == PersonaID);

            if (personaOriginal == null)
            {
                return (false, "Error: Persona no encontrada para actualizar.");
            }

            // Actualizar campos
            bool actualizacion = false;

            // Solo actualiza si hay un cambio
            if (personaOriginal.NombreCompleto != NombreCompleto)
            {
                personaOriginal.NombreCompleto = NombreCompleto;
                actualizacion = true;
            }

            if (personaOriginal.CorreoElectronico != CorreoElectronico)
            {
                personaOriginal.CorreoElectronico = CorreoElectronico;
                actualizacion = true;
            }

            if (personaOriginal.FechaNacimiento.Date != FechaNacimiento.Date)
            {
                personaOriginal.FechaNacimiento = FechaNacimiento;
                actualizacion = true;
            }

            if (actualizacion)
            {
                // Guardar cambios
                try
                {
                    _context.SaveChanges();
                    return (true, null);
                }
                catch (DbUpdateException)
                {
                    return (false, "Error: Falló la actualización de la Persona por un conflicto de datos.");
                }
                catch (Exception)
                {
                    return (false, "Error inesperado al actualizar los datos personales.");
                }
            }

            return (true, "Advertencia: No se detectaron cambios en los datos proporcionados.");
        }

        // CONSULTA PERSONA POR DNI
        public Persona? GetPersonaByDni(string Dni)
        {
            var persona = _context.Personas
                .AsNoTracking()
                .FirstOrDefault(p => p.Dni == Dni);

            return persona;
        }
    }
}
