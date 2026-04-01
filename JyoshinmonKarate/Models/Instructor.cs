using JyoshinmonKarate.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        [Required]
        public string UserId { get; set; }
        public int ClubId { get; set; }
        public int BeltId { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateJoined { get; set; }
        
        //One instructor can only have one user account
        public User User { get; set; }
        //A club can belong to many instructors
        public Club Club { get; set; }
        //A belt colour will be common for many instructors
        public Belt Belt { get; set; }
        //An instructor can teach many classes
        public ICollection<Schedule> Schedules { get; set; }
    }
}
