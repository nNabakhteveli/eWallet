using EWallet.Domain.Models;

namespace EBank;

public interface IEBank
{
    Task<int> ProcessDeposit(Deposit transaction, Deposit createdtransaction);
    Task<int> ProcessWithdraw(TransactionEntity transaction, TransactionEntity createdtransaction);
}