namespace WebEscuela.Repository.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string CorreoElectronico { get; set; }
        public int Dni { get; set; }
        public string Contrasenia { get; set; }
        public int RolId { get; set; }
        public int PersonaId { get; set; }


        // Navegación inversa
        public Rol Rol { get; set; } = null!;
        public Persona Persona { get; set; } = null!;
    }
}
