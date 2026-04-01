using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Belt
    {
        public int BeltId { get; set; }
        [Required]
        [StringLength(30)]
        public string BeltName { get; set; }
    }
}
