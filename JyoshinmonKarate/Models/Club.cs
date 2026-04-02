using System.ComponentModel.DataAnnotations;
namespace JyoshinmonKarate.Models
{
    public class Club
    {
        public int ClubId { get; set; }

        [Required(ErrorMessage = "Club name is required.")]
        [StringLength(100, ErrorMessage = "Club name cannot be more than 100 characters.")]
        [Display(Name = "Club Name")]
        public string ClubName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot be more than 200 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot be more than 100 characters.")]
        [Display(Name = "Club Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(30, ErrorMessage = "Phone number cannot be more than 30 characters.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        //One club can have many members
        public ICollection<Member> Members { get; set; }
        //One club can have many instructors
        public ICollection<Instructor> Instructors { get; set; }
        //One club can have many schedules
        public ICollection<Schedule> Schedules { get; set; }
        //One club can hold many gradings
        public ICollection<Grading> Gradings { get; set; }

    }
}
