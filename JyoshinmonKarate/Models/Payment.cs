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

        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Payment name is required.")]
        [StringLength(100, ErrorMessage = "Payment name cannot be more than 100 characters.")]
        [Display(Name = "Payment Name")]
        public string PaymentName { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Amount must be greater than 0 and less than 100000.")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "1/1/2200", ErrorMessage = "Date must be between 1900 and 2200")]
        [Display(Name = "Due Date")]
        public DateTime DateDue { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethods PaymentMethod { get; set; }

        [Display(Name = "Payment Status")]
        public PaymentStatus Status { get; set; }

        //A member can have many payments
        public Member Member { get; set; }
    }
}
