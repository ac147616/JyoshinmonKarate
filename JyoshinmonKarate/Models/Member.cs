using JyoshinmonKarate.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required(ErrorMessage = "A user account is required.")]
        [Display(Name = "User")]
        public string UserId { get; set; }

        [Display(Name = "Club")]
        public int ClubId { get; set; }

        [Display(Name = "Membership")]

        public int BeltId { get; set; }

        [Range(1, 20, ErrorMessage = "Belt size must be between 1 and 20.")]
        [Display(Name = "Belt Size")]
        public int BeltSize { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(30, ErrorMessage = "First name cannot be more than 30 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(30, ErrorMessage = "Last name cannot be more than 30 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [Range(typeof(DateTime), "01-01-1900", "01-01-2200", ErrorMessage = "Date must be between 1900 and 2200")]
        public DateTime DateOfBirth { get; set; }

        public string ProfilePhotoPath { get; set; }

        [NotMapped]
        public IFormFile ProfilePhotoFile { get; set; }

        public Gender Gender { get; set; }

        [Range(1, 500, ErrorMessage = "Weight must be between 1 and 500 kg.")]
        public int Weight { get; set; }

        [Range(1, 300, ErrorMessage = "Height must be between 1 and 300 cm.")]
        public int Height { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Joined")]
        [Range(typeof(DateTime), "1/1/1900", "1/1/2200", ErrorMessage = "Date must be between 1900 and 2200")]
        public DateTime DateJoined { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30, ErrorMessage = "Name cannot be more than 30 characters.")]
        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContactName { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(30, ErrorMessage = "Number cannot be more than 30 characters.")]
        [Display(Name = "Emergency Contact Phone")]
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
