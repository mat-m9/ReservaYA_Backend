using System.ComponentModel.DataAnnotations;

namespace ReservaYA_Backend.Models
{
    public class HorarioModel
    {
        [Key]
        public string? ID { get; set; }
        [Required]
        public string Desc { get; set; }
        [Required]
        public string Dia { get; set; }
        public bool? Cancha { get; set; } = true;
        public bool? Coliseo { get; set; } = true;

        public ICollection<ReservaInsModel> Instalaciones { get; set; }
    }
}
