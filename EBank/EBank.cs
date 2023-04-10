using EWallet.Domain.Models;
using EWallet.Domain.Data;

namespace EBank;

public class EBank : IEBank
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionsRepository _transactionsRepository;

    public EBank(IWalletRepository walletRepository, ITransactionsRepository transactionsRepository)
    {
        _walletRepository = walletRepository;
        _transactionsRepository = transactionsRepository;
    }

    public async Task<int> ProcessDeposit(Deposit transaction, Deposit createdtransaction)
    {
        int status = 1;
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);
        var recipientWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.RecipientId);

        if (userWallet.CurrentBalance < transaction.Amount || recipientWallet == null)
        {
            status = 2;
            return status;
        }

        userWallet.CurrentBalance -= transaction.Amount;
        recipientWallet.CurrentBalance += transaction.Amount;

        await _walletRepository.UpdateWalletAsync(userWallet);
        await _walletRepository.UpdateWalletAsync(recipientWallet);

        return status;
    }

    public async Task<int> ProcessWithdraw(TransactionEntity transaction, TransactionEntity createdTransaction)
    {
        var status = 1;
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);

        if (userWallet.CurrentBalance < transaction.Amount)
        {
            status = 2;
            createdTransaction.Status = status;

            userWallet.CurrentBalance += transaction.Amount;

            await _transactionsRepository.UpdateAsync(createdTransaction);
            await _walletRepository.UpdateWalletAsync(userWallet);

            return status;
        }

        return status;
    }
}