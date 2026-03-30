namespace JyoshinmonKarate.Models
{
    public class Grading
    {
        public int GradingId { get; set; }
        public int ClubId { get; set; }
        public DateTime GradingDate { get; set; }
        public DateTime GradingStartTime { get; set; }
        public DateTime GradingEndTime { get; set; }

        public Club Club { get; set; }
    }
}
