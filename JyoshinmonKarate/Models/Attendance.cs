namespace JyoshinmonKarate.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int ScheduleId { get; set; }
        public int MemberId { get; set; }
        public DateTime Date { get; set; }

        public Schedule Schedule { get; set; }
        public Member Member { get; set; }
    }
}
