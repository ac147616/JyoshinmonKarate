using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public enum Levels
    {
        Beginner, Intermediate, Advanced
    }

    public enum Weekdays
    {
        Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
    }

    public class Schedule
    {
        public int ScheduleId { get; set; }

        [Display(Name = "Club")]
        public int ClubId { get; set; }

        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [Display(Name = "Level")]
        public Levels Level { get; set; }

        [Display(Name = "Day of Week")]
        public Weekdays DayOfWeek { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        //One club will have many schedule/classes
        public Club Club { get; set; }
        //One club will have many instructors
        public Instructor Instructor { get; set; }
        //A class/schedule will have multiple attendances
        public ICollection<Attendance> Attendances { get; set; }
    }
}
