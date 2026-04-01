namespace JyoshinmonKarate.Models;
using System.ComponentModel.DataAnnotations;

public class Membership
{
    public int MembershipId { get; set; }
    [Required]
    [StringLength(50)]
    public string MembershipName { get; set; }
    public decimal Cost { get; set; }
    [Required]
    [StringLength(30)]
    public string AgeGroup { get; set; }

    //Many members will have the same membership plan
    public ICollection<Member> Members { get; set; }
}
