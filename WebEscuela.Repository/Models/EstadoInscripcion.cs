namespace WebEscuela.Repository.Models
{
    public class EstadoInscripcion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; }

    }
}
