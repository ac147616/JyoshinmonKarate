using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class MemberGrading
    {
        public int MemberGradingId { get; set; }
        public int GradingId { get; set; }
        public int MemberId { get; set; }
        public int BeltBeforeId { get; set; }
        public int BeltAfterId { get; set; }
        public bool Passed { get; set; }

        //One grading can have many member gradings to it
        public Grading Grading { get; set; }
        //One member can have attend gradings, thus many "member gradings"
        public Member Member { get; set; }
        //Many members/member gradings will have same belt before getting graded
        public Belt BeltBefore { get; set; }
        //Many members/member gradings will have same belt after getting graded
        public Belt BeltAfter { get; set; }


    }
}
