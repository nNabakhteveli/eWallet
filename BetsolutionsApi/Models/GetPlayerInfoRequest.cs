namespace BetsolutionsApi.Models;

public class GetPlayerInfoRequest
{
    public string Id { get; set; }
    public string Hash { get; set; }
    public string MerchantToken { get; set; }
    public string PrivateToken { get; set; }
}