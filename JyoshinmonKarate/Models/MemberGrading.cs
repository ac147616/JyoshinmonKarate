using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class MemberGrading
    {
        public int MembershipGradingId { get; set; }
        [Required]
        public int GradingId { get; set; }
        [Required]
        public int MembershipId { get; set; }
        [Required]
        public int BeltBeforeId { get; set; }
        [Required]
        public int BeltAfterId { get; set; }
        [Required]
        public bool Passed { get; set; }

        public Grading Grading { get; set; }
        public Membership Membership { get; set; }
        public Belt BeltBefore { get; set; }
        public Belt BeltAfter { get; set; }

    }
}
