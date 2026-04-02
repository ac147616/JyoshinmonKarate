using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Grading
    {
        public int GradingId { get; set; }

        [Display(Name = "Club")]
        public int ClubId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Grading Date")]
        public DateTime GradingDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Grading Date")]
        public DateTime GradingStartTime { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public DateTime GradingEndTime { get; set; }

        //One club can hold many gradings 
        public Club Club { get; set; }
        //One grading can include many member grading records
        public ICollection<MemberGrading> MemberGradings { get; set; }
    }
}