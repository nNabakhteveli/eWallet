using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace EWallet.Models;

public class WalletRepository : IWalletRepository
{
    private IDbConnection db;

    public WalletRepository(IConfiguration configuration)
    {
        db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }
    
    public async Task<WalletEntity> CreateWallet(WalletEntity wallet)
    {
        var sql =
            "INSERT INTO Wallets " +
            "(CurrentBalance)" +
            " VALUES (@CurrentBalance)" +
            "SET @Id = cast(scope_identity() as int) " +
            "SELECT SCOPE_IDENTITY() AS NewId";

        var newId = (int)(await db.QueryAsync(sql, wallet)).SingleOrDefault().NewId;
        
        wallet.Id = newId;
        
        return wallet;
    }

    public WalletEntity GetWalletById(int id)
    {
        return db.Query<WalletEntity>("SELECT Id, CurrentBalance FROM Wallets WHERE Id = @Id", new { id }).SingleOrDefault();
    }

    public WalletEntity UpdateWallet(WalletEntity wallet)
    {
        throw new NotImplementedException();
    }

    public void DeleteWallet(int id)
    {
        throw new NotImplementedException();
    }
}