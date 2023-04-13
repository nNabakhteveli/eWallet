using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface ITransactionsRepository
{
    Task<TransactionEntity> CreateAsync(TransactionEntity transaction);
    Task<Deposit> CreateAsync(Deposit transaction);
    
    Task<Deposit> UpdateAsync(Deposit transaction);
    Task<TransactionEntity> UpdateAsync(TransactionEntity transaction);
    Task<IEnumerable<Deposit>> GetAllAsync();
}