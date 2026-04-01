using JyoshinmonKarate.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ClubId { get; set; }
        [Required]
        public int BeltId { get; set; }
        [Required]
        public DateTime DateJoined { get; set; }

        public User User { get; set; }
        public Club Club { get; set; }
        public Belt Belt { get; set; }
    }
}
