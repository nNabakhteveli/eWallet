namespace EWallet.Domain.Models;

public class SeamlessBetRequest
{
    public Guid Token { get; set; }
    public decimal Amount { get; set; }
    // public int TransactionId { get; set; }
    public string Currency { get; set; }
    public string PaymentType { get; set; }
    public DateTime CreateDate { get; set; }
    public int Status { get; set; }
}