using ReservaYA_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReservaYA_Backend.ResponseModels
{
    public class RefreshToken
    {
        [Key]
        public string JwtId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Used { get; set; }
        public bool Ivalidated { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserModel User { get; set; }
    }
}
