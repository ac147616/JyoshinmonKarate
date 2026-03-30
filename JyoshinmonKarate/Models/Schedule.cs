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
        public int ClubId { get; set; }
        public int InstructorId { get; set; }
        public Levels Level { get; set; }
        public Weekdays DayOfWeek { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Club Club { get; set; }
        public Instructor Instructor { get; set; }
    }
}
