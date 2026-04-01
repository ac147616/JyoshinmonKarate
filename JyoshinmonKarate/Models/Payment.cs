using System.ComponentModel.DataAnnotations;

namespace JyoshinmonKarate.Models
{
    public enum PaymentMethods
    {
        CreditCard, BankTransfer, Cash, MobilePayment
    }

    public enum PaymentStatus
    {
        Pending, Completed, Failed
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int MemberId { get; set; }
        [Required]
        [StringLength(100)]
        public string PaymentName { get; set; }
        public decimal Amount { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateDue { get; set; }
        public PaymentMethods PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; }

        //A member can have many payments
        public Member Member { get; set; }
    }
}
