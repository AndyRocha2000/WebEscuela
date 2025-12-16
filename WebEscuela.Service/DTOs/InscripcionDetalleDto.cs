using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEscuela.Service.DTOs
{
    public class InscripcionDetalleDto
    {
        // Clave Compuesta
        public int AlumnoId { get; set; }
        public int MateriaId { get; set; }

        // Datos de la Inscripción
        public DateTime FechaInscripcion { get; set; }
        public int EstadoId { get; set; }
        public string TipoCursado { get; set; } = string.Empty;

        // Datos del Alumno
        public string AlumnoDni { get; set; } = string.Empty;
        public string AlumnoNombre { get; set; } = string.Empty;

        // Datos de la Materia
        public string MateriaNombre { get; set; } = string.Empty;
        public string DocenteAsignado { get; set; } = string.Empty;
        public string CarreraNombre { get; set; } = string.Empty;
    }
}
