namespace WebEscuela.Repository.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Contrasenia { get; set; } = string.Empty;
        public int RolId { get; set; }
        public int PersonaId { get; set; }
        public bool RequiereCambioContrasenia { get; set; } = true;

        // Propiedades de Navegacion
        public Rol Rol { get; set; } = null!;
        public Persona Persona { get; set; } = null!;
    }
}
