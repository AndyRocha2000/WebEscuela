namespace WebEscuela.Repository.Models
{
    public class Inscripcion
    {
        // Llaves Primarias
        public int AlumnoId { get; set; }
        public int MateriaId { get; set; }

        // Atributos
        public DateTime FechaInscripcion { get; set; }
        public int TipoCursadoId { get; set; }
        public int EstadoInscripcionId { get; set; }

        // Propiedades de Navegacion
        public Alumno Alumno { get; set; } = null!;
        public Materia Materia { get; set; } = null!;
        public TipoCursado TipoCursado { get; set; } = null!;
        public EstadoInscripcion EstadoInscripcion { get; set; } = null!;

    }
}
