using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public class Grading
    {
        public int GradingId { get; set; }
        public int ClubId { get; set; }
        [DataType(DataType.Date)]
        public DateTime GradingDate { get; set; }
        [DataType(DataType.Time)]
        public DateTime GradingStartTime { get; set; }
        [DataType(DataType.Time)]
        public DateTime GradingEndTime { get; set; }

        //One club can hold many gradings 
        public Club Club { get; set; }
        //One grading can include many member grading records
        public ICollection<MemberGrading> MemberGradings { get; set; }
    }
}