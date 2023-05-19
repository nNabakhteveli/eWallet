namespace EWallet.Domain.Models;

public class SeamlessCancelBetRequest : SeamlessBetRequest
{
    public int OldTransactionId { get; set; }
}