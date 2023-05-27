using System.ComponentModel.DataAnnotations;

namespace ReservaYA_Backend.Models
{
    public class ImplementoModel
    {
        [Key]
        public string? ID { get; set; }
        [Required]
        public string Desc { get; set; }
        [Required]
        public int Cant { get; set; }


        public ICollection<ReservaImpModel> Implementos { get; set; }
    }
}
