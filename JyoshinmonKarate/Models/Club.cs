using System.ComponentModel.DataAnnotations;
namespace JyoshinmonKarate.Models
{
    public class Club
    {
        public int ClubId { get; set; }
        [Required]
        [StringLength(100)]
        public string ClubName { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(30)]
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
