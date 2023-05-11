namespace BetsolutionsApi.Models.Transfers;

public class TransferRequest
{
    public decimal? Amount { get; set; }
    public int MerchantId { get; set; }
    public int TransactionId { get; set; }
    public Guid Token { get; set; }
    public string Key { get; set; }
    public string UserId { get; set; }
    public string? Hash { get; set; }
    public string Currency { get; set; }
}