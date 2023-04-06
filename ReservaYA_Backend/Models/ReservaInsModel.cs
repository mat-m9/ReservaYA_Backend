using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservaYA_Backend.Models
{
    public class ReservaInsModel
    {
        [Key]
        public string? ID { get; set; }
        [Required]
        public string Dia { get; set; }
        [Required]
        public string Tipo { get; set; }
        [Required]
        public string Hora { get; set; }
        
        public string Hor_ID { get; set; }
        [ForeignKey(nameof(Hor_ID))]
        public HorarioModel horario { get; set; }

        public string User_ID { get; set; }
        [ForeignKey(nameof(User_ID))]
        public UserModel User { get; set; }
    }
}
