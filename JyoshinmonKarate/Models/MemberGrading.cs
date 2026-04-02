using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JyoshinmonKarate.Models
{
    public class MemberGrading
    {
        public int MemberGradingId { get; set; }

        [Required(ErrorMessage = "Grading is required.")]
        [Display(Name = "Grading")]
        public int GradingId { get; set; }

        [Required(ErrorMessage = "Member is required.")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Previous belt is required.")]
        [Display(Name = "Belt Before")]
        public int BeltBeforeId { get; set; }

        [Required(ErrorMessage = "New belt is required.")]
        [Display(Name = "Belt After")]
        public int BeltAfterId { get; set; }

        [Display(Name = "Passed")]
        public bool Passed { get; set; }

        //One grading can have many member gradings to it
        public Grading Grading { get; set; }
        //One member can have attend gradings, thus many "member gradings"
        public Member Member { get; set; }
        //Many members/member gradings will have same belt before getting graded

        [ForeignKey("BeltBeforeId")]
        public Belt BeltBefore { get; set; }
        //Many members/member gradings will have same belt after getting graded
        [ForeignKey("BeltAfterId")]
        public Belt BeltAfter { get; set; }


    }
}
