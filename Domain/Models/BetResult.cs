namespace EWallet.Domain.Models;

public class BetResult
{
    public int TransactionId { get; set; }
    public decimal CurrentAmount { get; set; }
    public bool IsDuplicateTransaction { get; set; } = false;
}