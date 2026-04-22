namespace JyoshinmonKarate.Models;
using System.ComponentModel.DataAnnotations;

public class Membership
{
    public int MembershipId { get; set; }

    [Required(ErrorMessage = "Membership name is required.")]
    [StringLength(50, ErrorMessage = "Membership name cannot be more than 50 characters.")]
    [Display(Name = "Membership Name")]
    public string MembershipName { get; set; }

    [Range(0.01, 10000, ErrorMessage = "Cost must be greater than 0 and less than 10000.")]
    [Display(Name = "Membership Cost")]
    public decimal Cost { get; set; }

    [Required(ErrorMessage = "Group is required.")]
    [StringLength(30, ErrorMessage = "Group cannot be more than 30 characters.")]
    [Display(Name = "Group")]
    public string AgeGroup { get; set; }

    //Many members will have the same membership plan
    public ICollection<MemberMembership> MemberMemberships { get; set; }
}
