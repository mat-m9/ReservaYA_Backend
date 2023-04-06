using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReservaYA_Backend.Models
{
    public class ReservaImpModel
    {
        [Key]
        public string? ID { get; set; }
        [Required]
        public string Dia { get; set; }
        [Required]
        public string Tipo { get; set; }

        public string Imp_ID { get; set; }
        [ForeignKey(nameof(Imp_ID))]
        public HorarioModel implemento { get; set; }


        public string User_ID { get; set; }
        [ForeignKey(nameof(User_ID))]
        public UserModel User { get; set;}
    }
}
