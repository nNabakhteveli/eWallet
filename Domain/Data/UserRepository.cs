using System.Data;
using Dapper;
using EWallet.Domain.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Domain.Data;

public class UserRepository : IUserRepository
{
    private IDbConnection db;

    public UserRepository(string connectionString)
    {
        db = new SqlConnection(connectionString);
    }
    
    
    public async Task<dynamic> GetUserByIdAsync(string id)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@UserId", id);

        return (await db.QueryAsync("GetUserById", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
    }
    
    // public async Task<UserEntity> GetUserByIdAsync(string id)
    // {
    //     var parameters = new DynamicParameters();

    //     parameters.Add("@UserId", id);
         
    //     return (await db.QueryAsync<UserEntity>("GetUserById", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
    // }
}