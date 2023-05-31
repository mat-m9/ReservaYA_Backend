using Microsoft.AspNetCore.Identity;

namespace ReservaYA_Backend.Models
{
    public class UserModel : IdentityUser
    {

        public int NumReservas { get; set; } = 0;
        public ICollection<ReservaImpModel> Implementos { get; set; }
        public ICollection<ReservaInsModel> Instalaciones { get; set; }
    }
}
