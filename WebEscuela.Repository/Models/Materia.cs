namespace WebEscuela.Repository.Models
{
    public class Materia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Cuatrimestre { get; set; }
        public int CupoMaximo { get; set; }

        // Llaves foraneas
        public int CarreraId { get; set; }
        public int? DocenteId { get; set; }

        // Propiedades de Navegacion
        public Docente? Docente { get; set; } = null!;
        public Carrera Carrera { get; set; } = null!;

        // Navegacion inversa
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
