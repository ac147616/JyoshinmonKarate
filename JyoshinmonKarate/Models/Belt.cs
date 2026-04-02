using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Belt
    {
        public int BeltId { get; set; }

        [Required(ErrorMessage = "Belt name is required.")]
        [StringLength(30, ErrorMessage = "Belt name cannot be more than 30 characters.")]
        [Display(Name = "Belt Name")]
        public string BeltName { get; set; }

        //One belt colour can be used by many members
        public ICollection<Member> Members { get; set; }
        //One belt colour can be used by many instructors
        public ICollection<Instructor> Instructors { get; set; }
        //Many member gradings will have members whos belts were the same colour before grading
        public ICollection<MemberGrading> BeltBeforeMemberGradings { get; set; }
        //Many member gradings will have members whos belts become the same colour after grading
        public ICollection<MemberGrading> BeltAfterMemberGradings { get; set; }
    }
}
