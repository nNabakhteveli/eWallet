namespace EWallet.Domain.Models;

public class TokenEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public Guid PublicToken { get; set; }
    public string PublicTokenStatus { get; set; } = "active";
    public Guid PrivateToken { get; set; }
    public string PrivateTokenStatus { get; set; } = "active";
}