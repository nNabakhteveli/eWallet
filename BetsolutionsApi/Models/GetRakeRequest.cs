namespace BetsolutionsApi.Models;

public class GetRakeRequest
{
    public string UserId { get; set; }
    public int MerchantId { get; set; }
    public string? Hash { get; set; }
    public int GameId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}