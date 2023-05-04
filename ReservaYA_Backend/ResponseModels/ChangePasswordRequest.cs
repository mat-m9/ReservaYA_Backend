namespace ReservaYA_Backend.ResponseModels
{
    public class ChangePasswordRequest
    {
        public string Mail { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
