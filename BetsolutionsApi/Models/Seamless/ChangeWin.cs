namespace BetsolutionsApi.Models.Seamless;

public class ChangeWin : SeamlessBaseRequest
{
    public decimal PreviousAmount { get; set; }
    public int TransactionId { get; set; }
    public int ChangeWinTypeId { get; set; }
}