namespace WebEscuela.Repository.Models
{
    public class Alumno : Persona
    {
        public int CarreraId { get; set; }

        // Propiedades de Navegacion
        public Carrera Carrera { get; set; } = null!;

        // Navegacion inversa
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

    }
}
