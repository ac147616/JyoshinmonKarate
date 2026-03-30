namespace JyoshinmonKarate.Models
{
    public enum MemberStatus
    {
        Active, Inactive, Suspended
    }
    public enum Gender {
        Female, Male, PreferNotToSay
    }

    public class Member
    {
        public int MemberId { get; set; }
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public int MembershipId { get; set; }
        public int BeltId { get; set; }
        public int BeltSize { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public DateOnly DateJoined { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public MemberStatus Status { get; set; }

        public User User { get; set; }
        public Club Club { get; set; }
        public Membership Membership { get; set; }
        public Belt Belt { get; set; }
    }
}
