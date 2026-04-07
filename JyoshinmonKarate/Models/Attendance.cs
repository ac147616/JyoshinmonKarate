using System;
using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Attendance
    {
        // I didn't add required for int and dates because they are already non-nullable
        public int AttendanceId { get; set; }

        [Display(Name = "Schedule")]
        public int ScheduleId { get; set; }

        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Attendance Date")]
        [Range(typeof(DateTime), "1/1/1900", "1/1/2200", ErrorMessage = "Date must be between 1900 and 2200")]
        public DateTime Date { get; set; }

        //one schedule can have many attendances
        public Schedule Schedule { get; set; }
        //one member can have many attendances
        public Member Member { get; set; }
    }
}