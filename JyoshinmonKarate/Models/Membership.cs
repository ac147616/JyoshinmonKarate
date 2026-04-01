namespace JyoshinmonKarate.Models;
using System.ComponentModel.DataAnnotations;

public class Membership
{
    public int MembershipId { get; set; }
    [Required]
    public string MembershipName { get; set; }
    [Required]
    public decimal Cost { get; set; }
    [Required]
    public string AgeGroup { get; set; }
}
