namespace WebEscuela.Repository.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }

        // Propiedades de Navegacion
        public Usuario Usuario { get; set; } = null!;
    }
}
