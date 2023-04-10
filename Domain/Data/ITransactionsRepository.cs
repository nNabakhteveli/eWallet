using EWallet.Domain.Models;

namespace EWallet.Data;

public interface ITransactionsRepository
{
    Task<TransactionEntity> CreateAsync(TransactionEntity transaction);
}