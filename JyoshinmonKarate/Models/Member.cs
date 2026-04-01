using JyoshinmonKarate.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public enum MemberStatus
    {
        Active, Inactive, Suspended
    }
    public enum Gender {
        Female, Male, PreferNotToSay
    }
    public class Member
    {
        public int MemberId { get; set; }
        [Required]
        public string UserId { get; set; }
        public int ClubId { get; set; }
        public int MembershipId { get; set; }
        public int BeltId { get; set; }
        public int BeltSize { get; set; }
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateJoined { get; set; }
        [StringLength(30)]
        public string EmergencyContactName { get; set; }
        [Phone]
        [StringLength(30)]
        public string EmergencyContactPhone { get; set; }
        public MemberStatus Status { get; set; }

        //A user can manage many members
        public User User { get; set; }
        //A club has many club members
        public Club Club { get; set; }
        //Many members will be on the same membership plan
        public Membership Membership { get; set; }
        //A belt colour would be common for many members
        public Belt Belt { get; set; }
        //A member can have many attendances
        public ICollection<Attendance> Attendances { get; set; }
        //A member will have many payments to make
        public ICollection<Payment> Payments { get; set; }
        //A member will attend many member gradings
        public ICollection<MemberGrading> MemberGradings { get; set; }
    }
}
