using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Grading
    {
        public int GradingId { get; set; }
        [Required]
        public int ClubId { get; set; }
        [Required]
        public DateTime GradingDate { get; set; }
        [Required]
        public DateTime GradingStartTime { get; set; }
        [Required]
        public DateTime GradingEndTime { get; set; }

        public Club Club { get; set; }
    }
}
