namespace JyoshinmonKarate.Models
{
    public class MemberGrading
    {
        public int MembershipGradingId { get; set; }
        public int GradingId { get; set; }
        public int MembershipId { get; set; }
        public int BeltBeforeId { get; set; }
        public int BeltAfterId { get; set; }
        public bool Passed { get; set; }

        public Grading Grading { get; set; }
        public Membership Membership { get; set; }
        public Belt BeltBefore { get; set; }
        public Belt BeltAfter { get; set; }

    }
}
