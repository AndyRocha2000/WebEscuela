namespace WebEscuela.Repository.Models
{
    public class Carrera
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public int DuracionAnios { get; set; }
        public string TituloOtorgado { get; set; } = string.Empty;

        // Llaves foraneas
        public int ModalidadId { get; set; }
        public int TurnoId { get; set; }
        public int? PreceptorId { get; set; }

        // Propiedades de Navegacion
        public Modalidad Modalidad { get; set; } = null!;
        public Turno Turno { get; set; } = null!;
        public Usuario? Preceptor { get; set; } = null!;

        // Navegacion inversa
        public ICollection<Materia> Materias { get; set; } = new List<Materia>();
        public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();

    }
}
