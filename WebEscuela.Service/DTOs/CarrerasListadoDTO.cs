namespace WebEscuela.Service.DTOs
{
    public class CarrerasListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public int DuracionAnios { get; set; }
        public string TituloOtorgado { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
        public string Turno { get; set; } = string.Empty;
        public string Preceptor { get; set; } = string.Empty;
    }
}
