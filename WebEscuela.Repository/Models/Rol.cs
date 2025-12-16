namespace WebEscuela.Repository.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Navegacion inversa
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
