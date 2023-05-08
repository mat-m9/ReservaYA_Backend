namespace ReservaYA_Backend.ResponseModels
{
    public class ReservaRequest
    {
        public string Dia { get; set; }
        public string? Hora { get; set; }
        public string Tipo { get; set; }
        public string UsuarioID { get; set;}
    }
}
