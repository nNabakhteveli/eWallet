using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface ITokenRepository
{
    Task<TokenEntity> CreateAsync(TokenEntity tokenData);
    Task<TokenEntity> UpdateAsync(TokenEntity tokenData);
    Task<TokenEntity> GetByUserIdAsync(string id);
    Task<TokenEntity> GetByPrivateToken(Guid token);
    Task<TokenEntity> GetByPublicToken(Guid token);
    Task DeleteAsync(TokenEntity token);
    Task DeleteAsync(Guid userId);
}