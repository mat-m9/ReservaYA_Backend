namespace ReservaYA_Backend.ResponseModels
{
    public class ResponseModel
    {
        public string Tipo { get; set; }
        public string Message { get; set; }
        public string? token { get; set; }
        public string? refreshedToken { get; set; }
    }
}
