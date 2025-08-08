namespace WebEscuela.Repository.Models
{
    public class Materia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Anio { get; set; }
        public int Cuatrimestre { get; set; }
        public int CupoMaximo { get; set; }
        public int CarreraId { get; set; }
        public int DocenteId { get; set; }

        public Docente Docente { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; }
    }
}
