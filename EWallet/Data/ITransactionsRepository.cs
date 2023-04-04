namespace EWallet.Models;

public interface ITransactionsRepository
{
    Task<TransactionEntity> CreateAsync(TransactionEntity transaction);
}