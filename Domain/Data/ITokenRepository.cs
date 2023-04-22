using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface ITokenRepository
{
    Task<TokenEntity> CreateAsync(TokenEntity tokenData);
    Task<TokenEntity> GetByUserIdAsync(string id);
    Task<TokenEntity> GetByUserToken(Guid token);
    Task DeleteAsync(TokenEntity token);
    Task DeleteAsync(Guid userId);
}