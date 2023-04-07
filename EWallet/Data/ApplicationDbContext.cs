using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EWallet.Domain.Models;

namespace EWallet.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<TransactionEntity> Transactions { get; set; }
}