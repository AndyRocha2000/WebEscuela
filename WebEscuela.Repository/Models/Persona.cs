namespace WebEscuela.Repository.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public int Dni { get; set; }
        public string CorreoElectronico { get; set; }
        public DateTime FechaNacimiento { get; set; }
        
        // Navegación inversa
        public Usuario Usuario { get; set; } = null!;
    }
}
