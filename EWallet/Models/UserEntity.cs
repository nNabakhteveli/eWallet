using Microsoft.AspNetCore.Identity;

namespace EWallet.Models;

public class UserEntity : IdentityUser
{
    public int Id { get; set; }
    public int WalletId { get; set; }
    public string Password { get; set; }
}