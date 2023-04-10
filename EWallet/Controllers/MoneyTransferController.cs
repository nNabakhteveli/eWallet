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
    private string errorPageUrl = "/InvalidRequest";

    public MoneyTransferController(ITransactionsRepository transactionsRepository, IWalletRepository walletRepository, IEBank bank)
    {
        _transactionsRepository = transactionsRepository;
        _walletRepository = walletRepository;
        _bank = bank;
    }

    [HttpPost]
    [Route("/Transactions/Deposit")]
    public async Task<IActionResult> Deposit(Deposit transaction)
    {
        transaction.PaymentType = "Deposit";
        if (ModelState.IsValid)
        {
            var createdTransaction = await _transactionsRepository.CreateAsync(transaction);
            
            // imitation of a bank
            var transactionStatus = await _bank.ProcessDeposit(transaction, createdTransaction);
            createdTransaction.Status = transactionStatus;
            
            await _transactionsRepository.UpdateAsync(createdTransaction);
            
            return Redirect("/Identity/Account/Wallet");
        }
    
        transaction.Status = 2;
    
        await _transactionsRepository.CreateAsync(transaction);
        return Redirect(errorPageUrl);
    }
    
    [HttpPost]
    [Route("/Transactions/Withdraw")]
    public async Task<IActionResult> Withdraw(TransactionEntity transaction)
    {
        transaction.PaymentType = "Withdraw";
        if (ModelState.IsValid)
        {
            var createdTransaction = await _transactionsRepository.CreateAsync(transaction);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);
            
            userWallet.CurrentBalance -= transaction.Amount;

            await _walletRepository.UpdateWalletAsync(userWallet);
            
            var transactionStatus = await _bank.ProcessWithdraw(transaction, createdTransaction);
            createdTransaction.Status = transactionStatus;

            await _transactionsRepository.UpdateAsync(createdTransaction);
            
            return Redirect("/Identity/Account/Withdraw");
        }

        transaction.Status = 2;

        await _transactionsRepository.CreateAsync(transaction);
        return Redirect(errorPageUrl);
    }
}