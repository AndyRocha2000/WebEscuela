namespace WebEscuela.Repository.Models
{
    public class Alumno : Persona
    {
        public int CarreraId { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; }

    }
}
