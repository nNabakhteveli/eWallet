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
        var parameters = new DynamicParameters();

        parameters.Add("@Id", value: wallet.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@CurrentBalance", wallet.CurrentBalance);

        var newId = (await db.QueryAsync<int>("CreateWallet", parameters, commandType: CommandType.StoredProcedure))
            .SingleOrDefault();
        
        wallet.Id = newId;
        
        return wallet;
    }

    public WalletEntity GetWalletById(int id)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@Id", value: id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        
        return db.Query<WalletEntity>("GetWalletById", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
    }
    
    public async Task<WalletEntity> GetWalletByUserIdAsync(string id)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@UserId", value: id);
        
        return (await db.QueryAsync<WalletEntity>("GetWalletByUserId", parameters, commandType: CommandType.StoredProcedure)).SingleOrDefault();
    }

    public async Task<WalletEntity> UpdateWalletAsync(WalletEntity wallet)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@Id", value: wallet.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@CurrentBalance", wallet.CurrentBalance);

        await db.ExecuteAsync("UpdateWallet", parameters, commandType: CommandType.StoredProcedure);

        return wallet;
    }
}