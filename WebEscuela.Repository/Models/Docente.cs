namespace WebEscuela.Repository.Models
{
    public class Docente : Persona
    {
        public string Titulo { get; set; }

        public ICollection<Materia> Materias { get; set; }

    }
}
