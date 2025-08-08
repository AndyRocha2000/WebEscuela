namespace WebEscuela.Repository.Models
{
    public class Carrera
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        public int DuracionAnios { get; set; }
        public string TituloOtorgado { get; set; }

        public int PreceptorId { get; set; }
        public Usuario Preceptor { get; set; }

    }
}
