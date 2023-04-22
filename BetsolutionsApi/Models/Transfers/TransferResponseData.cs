namespace BetsolutionsApi.Models.Transfers;

public class TransferResponseData
{
    public int TransactionId { get; set; }
    public decimal Balance { get; set; }
    
    public decimal CurrentBalance { get; set; }
}