namespace WebEscuela.Repository.Models
{
    public class Modalidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Navegacion inversa
        public ICollection<Carrera> Carreras { get; set; } = new List<Carrera>();
    }
}
