using JyoshinmonKarate.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "A user account is required.")]
        [Display(Name = "User")]
        public string UserId { get; set; }

        [Display(Name = "Club")]
        public int ClubId { get; set; }

        [Display(Name = "Belt")]
        public int BeltId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Joined")]
        [Range(typeof(DateTime), "1/1/1900", "1/1/2200", ErrorMessage = "Date must be between 1900 and 2200")]
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
