using System.Data;
using Dapper;
using EWallet.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EWallet.Domain.Data;

public class WalletRepository : IWalletRepository
{
    private IDbConnection db;

    public WalletRepository(string connectionString)
    {
        db = new SqlConnection(connectionString);
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
    
    public async Task<WalletEntity> GetWalletByUserIdAsync(string id)
    {
        return (await db.QueryAsync<WalletEntity>("SELECT Id, UserId, CurrentBalance FROM Wallets WHERE UserId = @Id", new { id })).SingleOrDefault();
    }

    public async Task<WalletEntity> UpdateWalletAsync(WalletEntity wallet)
    {
        var sql =
            "UPDATE Wallets " +
            "SET UserId = @UserId, " +
            "CurrentBalance = @CurrentBalance " +
            "WHERE Id = @Id";

        await db.ExecuteAsync(sql, wallet);

        return wallet;
    }

    public void DeleteWallet(int id)
    {
        throw new NotImplementedException();
    }
}