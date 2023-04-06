using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ReservaYA_Backend.Models;

namespace ReservaYA_Backend.ResponseModels
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        public string RefreshedToken { get; set; }
    }
}
