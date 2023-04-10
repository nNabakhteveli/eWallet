using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface IWalletRepository
{
    Task<WalletEntity> CreateWallet(WalletEntity wallet);
    WalletEntity GetWalletById(int id);
    Task<WalletEntity> GetWalletByUserIdAsync(string id);
    Task<WalletEntity> UpdateWalletAsync(WalletEntity wallet);
    void DeleteWallet(int id);
}