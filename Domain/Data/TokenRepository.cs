using System.Data;
using System.Data.Common;
using Dapper;
using EWallet.Domain.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Domain.Data;

public class TokenRepository : ITokenRepository
{
    private readonly DbConnection db;

    public TokenRepository(string connectionString)
    {
        db = new SqlConnection(connectionString);
    }

    public async Task<TokenEntity> CreateAsync(TokenEntity tokenData)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@Id", value: tokenData.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@UserId", tokenData.UserId);
        parameters.Add("@PublicToken", tokenData.PublicToken);
        parameters.Add("@PublicTokenStatus", tokenData.PublicTokenStatus);
        parameters.Add("@PrivateToken", tokenData.PrivateToken);
        parameters.Add("@PrivateTokenStatus", tokenData.PrivateTokenStatus);
        
        var newId = (await db.QueryAsync<int>("CreateToken", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        
        tokenData.Id = newId;

        return tokenData;
    }

    public async Task<TokenEntity> GetByUserToken(Guid token)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@PrivateToken", token);

        return (await db.QueryAsync<TokenEntity>("GetTokenByPrivateToken", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
    }

    public async Task<TokenEntity> GetByUserIdAsync(string id)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@UserId", id);

        return (await db.QueryAsync<TokenEntity>("GetTokenByUserId", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
    }

    public async Task DeleteAsync(Guid userId)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@UserId", userId);
        
        await db.QueryAsync<TokenEntity>("DeleteTokenByUserId", parameters, commandType: CommandType.StoredProcedure);
    }
    
    public async Task DeleteAsync(TokenEntity token)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("@PrivateToken", token);
        
        await db.QueryAsync<TokenEntity>("DeleteTokenByPrivateToken", parameters, commandType: CommandType.StoredProcedure);
    }
}