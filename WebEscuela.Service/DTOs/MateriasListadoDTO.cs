namespace WebEscuela.Service.DTOs
{
    public class MateriasListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Cuatrimestre { get; set; }
        public int CupoMaximo { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
    }
}
