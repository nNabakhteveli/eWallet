namespace BetsolutionsApi.Models.Seamless;

public class SeamlessBaseRequest
{
    public Guid Token { get; set; }
    public decimal Amount { get; set; }
    public int TransactionId { get; set; }
    public int GameId { get; set; }
    public int ProductId { get; set; }
    public string MerchantToken { get; set; }
    public int RoundId { get; set; }
    public string Hash { get; set; }
    public string Currency { get; set; }
    public int CampaignId { get; set; }
    public string CampaignName { get; set; }
}