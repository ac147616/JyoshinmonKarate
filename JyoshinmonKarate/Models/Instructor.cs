using JyoshinmonKarate.Areas.Identity.Data;

namespace JyoshinmonKarate.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public int BeltId { get; set; }
        public DateTime DateJoined { get; set; }

        public User User { get; set; }
        public Club Club { get; set; }
        public Belt Belt { get; set; }
    }
}
