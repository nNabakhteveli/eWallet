using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface ITransactionsRepository
{
    Task<IEnumerable<TransactionEntity>> FilterInRange(string startDate, string endDate);
    Task<TransactionEntity> CreateAsync(TransactionEntity transaction);
    Task<TransactionEntity> UpdateAsync(TransactionEntity transaction);
    Task<IEnumerable<TransactionEntity>> GetAllAsync();
    Task<TransactionEntity> GetTransactionByIdAsync(string id);
}