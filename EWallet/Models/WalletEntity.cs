namespace EWallet.Models;

public class WalletEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public decimal CurrentBalance { get; set; } = 0;
}