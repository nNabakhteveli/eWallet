namespace BetsolutionsApi.Models.Seamless;

public class CancelBet : SeamlessBaseRequest
{
    public int BetTypeId { get; set; }
    public int BetTransactionId { get; set; }
}