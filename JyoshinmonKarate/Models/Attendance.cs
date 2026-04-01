using System.ComponentModel.DataAnnotations;
namespace JyoshinmonKarate.Models;
{
    public class Attendance
    {
        [Required]
        public int AttendanceId { get; set; }
        [Required]
        public int ScheduleId { get; set; }
        [Required]
        public int MemberId { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public Schedule Schedule { get; set; }
        public Member Member { get; set; }
    }
}
