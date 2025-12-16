namespace WebEscuela.Repository.Models
{
    public class Docente : Persona
    {
        public string Titulo { get; set; } = string.Empty;

        // Navegacion inversa
        public ICollection<Materia> Materias { get; set; } = new List<Materia>();

    }
}
