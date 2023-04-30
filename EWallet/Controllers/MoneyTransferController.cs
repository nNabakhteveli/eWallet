using EBank;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class MoneyTransferController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IEBank _bank;

    public MoneyTransferController(ITransactionsRepository transactionsRepository, IWalletRepository walletRepository,
        IEBank bank)
    {
        _transactionsRepository = transactionsRepository;
        _walletRepository = walletRepository;
        _bank = bank;
    }

    [HttpPost]
    [Route("/Transactions/Api/Deposit")]
    public async Task<JsonResult> Deposit(TransactionEntity transaction)
    {
        transaction.CreateDate = DateTime.Now;

        if (!ModelState.IsValid || transaction.Amount < 0)
        {
            transaction.Status = 2;

            await _transactionsRepository.CreateAsync(transaction);
            return Json(new { success = false });
        }

        var createdTransaction = await _transactionsRepository.CreateAsync(transaction);

        // imitation of a bank
        var transactionStatus = _bank.ValidateTransfer();
        createdTransaction.Status = transactionStatus;
        
        await _transactionsRepository.UpdateAsync(createdTransaction);
        
        var success = false;
        if (transactionStatus == 1)
        {
            success = true;
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);

            userWallet.CurrentBalance += transaction.Amount;
            await _walletRepository.UpdateWalletAsync(userWallet);
        }

        return Json(new { success });
    }

    [HttpPost]
    [Route("/Transactions/Api/Withdraw")]
    public async Task<JsonResult> Withdraw(TransactionEntity transaction)
    {
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);
        transaction.CreateDate = DateTime.Now;

        if (!ModelState.IsValid || transaction.Amount > userWallet.CurrentBalance)
        {
            transaction.Status = 2;

            await _transactionsRepository.CreateAsync(transaction);
            return Json(new { success = false });
        }

        var createdTransaction = await _transactionsRepository.CreateAsync(transaction);

        userWallet.CurrentBalance -= transaction.Amount;

        await _walletRepository.UpdateWalletAsync(userWallet);

        var transactionStatus = _bank.ValidateTransfer();
        createdTransaction.Status = transactionStatus;

        await _transactionsRepository.UpdateAsync(createdTransaction);

        var success = true;
        
        if (transactionStatus == 2)
        {
            success = false;
            
            userWallet.CurrentBalance += transaction.Amount;
            await _walletRepository.UpdateWalletAsync(userWallet);
        }

        return Json(new { success });
    }
}