using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public enum MembershipStatus
    {
        Active,    //still going
        Suspended, //end date passed
        Cancelled  //The membership was cancelled before the end date for some reason
    }
    public class MemberMembership
    {
        public int MemberMembershipId { get; set; }

        [Display(Name = "Member")]
        public int MemberId { get; set; }   

        [Display(Name = "Membership")]
        public int MembershipId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Membership Status")]
        public MembershipStatus MembershipStatus { get; set; }

        //A member can have many memberships over time
        public Member Member { get; set; }
        //A membership can be used by many members over time
        public Membership Membership { get; set; }
    }
}
