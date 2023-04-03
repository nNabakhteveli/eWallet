namespace EWallet.Models;

public interface IWalletRepository
{
    Task<WalletEntity> CreateWallet(WalletEntity wallet);
    WalletEntity GetWalletById(int id);
    WalletEntity UpdateWallet(WalletEntity wallet);
    void DeleteWallet(int id);
}