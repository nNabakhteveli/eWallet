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
        var sql =
            "INSERT INTO Tokens " +
            "(UserId, PublicToken, PublicTokenStatus, PrivateToken, PrivateTokenStatus)" +
            " VALUES (@UserId, @PublicToken, @PublicTokenStatus, @PrivateToken, @PrivateTokenStatus) " +
            "SET @Id = cast(scope_identity() as int) " +
            "SELECT SCOPE_IDENTITY() AS NewId";

        var newId = (int)(await db.QueryAsync(sql, tokenData)).SingleOrDefault().NewId;

        tokenData.Id = newId;

        return tokenData;
    }

    public async Task<TokenEntity> GetByUserToken(Guid token)
    {
        return (await db.QueryAsync<TokenEntity>("SELECT * FROM Tokens WHERE PrivateToken = @Token", new { token })).FirstOrDefault();
    }

    public async Task<TokenEntity> GetByUserIdAsync(string id)
    {
        return (await db.QueryAsync<TokenEntity>("SELECT * FROM Tokens WHERE UserId = @Id", new { id })).FirstOrDefault();
    }

    public async Task DeleteAsync(Guid userId)
    {
        await db.QueryAsync<TokenEntity>("DELETE FROM Tokens Where UserId = @UserId", new { UserId = userId });
    }
    
    public async Task DeleteAsync(TokenEntity token)
    {
        await db.QueryAsync<TokenEntity>("DELETE FROM Tokens Where Id = @Id", new { token.Id });
    }
}