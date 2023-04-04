namespace EWallet.Models;

public class TransactionEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string? RecipientId { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    // 0 - pending, 1 - success, 2 - fail
    public int Status { get; set; } = 0;
}