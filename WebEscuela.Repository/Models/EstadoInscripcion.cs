namespace WebEscuela.Repository.Models
{
    public class EstadoInscripcion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Navegacion inversa
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

    }
}
