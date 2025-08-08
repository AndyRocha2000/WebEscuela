namespace WebEscuela.Repository.Models
{
    public class Inscripcion
    {
        public int AlumnoId { get; set; }
        public int MateriaId { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public int EstadoInscripcionId { get; set; }

        public Alumno Alumno { get; set; }
        public Materia Materia { get; set; }
        public EstadoInscripcion EstadoInscripcion { get; set; }

    }
}
