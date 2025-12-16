namespace WebEscuela.Repository.Models
{
    public class TipoCursado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Navegación inversa
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}