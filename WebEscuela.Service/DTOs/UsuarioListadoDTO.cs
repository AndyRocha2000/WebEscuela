namespace WebEscuela.Service.DTOs
{
    public class UsuarioListadoDTO
    {
        public int Id { get; set; }

        // Estas propiedades son las que veremos en la tabla
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
    }
}
