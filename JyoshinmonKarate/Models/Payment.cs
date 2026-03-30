namespace JyoshinmonKarate.Models
{
    public enum PaymentMethods
    {
        CreditCard, BankTransfer, Cash, MobilePayment
    }

    public enum Status
    {
        Pending, Completed, Failed
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int MemberId { get; set; }
        public string PaymentName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateDue { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public Status Status { get; set; }

        public Member Member { get; set; }
    }
}
