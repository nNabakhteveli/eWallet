namespace EWallet.Domain.Models;

public class Deposit : TransactionEntity
{
    public string? RecipientId { get; set; }
}